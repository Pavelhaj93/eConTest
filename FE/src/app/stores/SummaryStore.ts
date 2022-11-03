import { QueryParams } from '@types'
import { parseUrl } from '@utils'
import { action, observable } from 'mobx'
import { SumarryErrorResponse, SummaryResponse } from '../types/Summary'

export class SummaryStore {
  private globalQueryParams: QueryParams

  @observable
  public error = false

  @observable
  public errorMessage = ''

  @observable
  public isLoading = false

  @observable
  public forceReload = false

  @observable
  public personalDetails: SummaryResponse.Personal | undefined = undefined

  @observable
  public distributorChange: SummaryResponse.DistributorChange | undefined = undefined

  @observable
  public product: SummaryResponse.Product | undefined = undefined

  @observable
  public benefits: SummaryResponse.Benefits[] | undefined = undefined

  @observable
  public gifts: SummaryResponse.Gifts | undefined = undefined

  constructor(guid: string, public summaryUrl: string, public errorPageUrl: string) {
    this.summaryUrl = summaryUrl
    this.errorPageUrl = errorPageUrl
    this.globalQueryParams = { guid }
  }

  @action public async fetchSummary(timeoutMs?: number): Promise<void> {
    this.isLoading = true

    try {
      let fetchTimeout: NodeJS.Timeout | number | null = null
      let controller: AbortController | null = null

      // if timeoutMs is present => cancel the fetch request after this value
      if (timeoutMs) {
        controller = new AbortController()
        fetchTimeout = setTimeout(() => {
          controller && controller.abort()
        }, timeoutMs)
      }

      const response = await fetch(parseUrl(this.summaryUrl, this.globalQueryParams), {
        headers: { Accept: 'application/json' },
        signal: controller?.signal || null,
      })

      fetchTimeout && clearTimeout(fetchTimeout)

      // redirect to error page when 404 response
      if (response.status === 404) {
        this.forceReload = true
        window.location.href = this.errorPageUrl
        return
      }

      // handle 5xx statuses and custom error message
      if (response.status.toString().startsWith('5')) {
        const { Message } = await (response.json() as Promise<SumarryErrorResponse>)
        this.errorMessage = Message
        throw new Error(Message)
      }

      // the rest of the statuses are treated as unknown errors
      if (!response.ok) {
        throw new Error(`FAILED TO FETCH OFFER - ${response.status}`)
      }

      const jsonResponse = await (response.json() as Promise<SummaryResponse.RootObject>)

      this.personalDetails = jsonResponse.personal
      this.distributorChange = jsonResponse.distributor_change
      this.product = jsonResponse.product
      this.benefits = jsonResponse.benefits
      this.gifts = jsonResponse.gifts
    } catch (error) {
      // eslint-disable-next-line no-console
      console.error(String(error))
      this.error = true
    } finally {
      this.isLoading = false
    }
  }
}
