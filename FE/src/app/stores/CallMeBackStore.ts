import { action, observable } from 'mobx'
import {
  CallMeBackErrorResponse,
  ICallMeBackForm,
  IGetCallMeBackResponse,
  IPostCallMeBackResponse,
} from '../types/CallMeBack'
import { parseUrl } from '@utils'
import { QueryParams } from '@types'

export class CallMeBackStore {
  private globalQueryParams: QueryParams

  @observable
  public error = false

  @observable
  public errorMessage = ''

  @observable
  public isLoading = false

  @observable
  public isSuccess = false

  @observable
  public getResponseDetails: IGetCallMeBackResponse | undefined = undefined

  @observable
  public postResponseDetails: IPostCallMeBackResponse | undefined = undefined

  constructor(guid: string, public responseUrl: string) {
    this.responseUrl = responseUrl
    this.globalQueryParams = { guid }
  }

  /* Set error value. */
  @action public setError(error: boolean): void {
    this.error = error
  }

  /* Get data for form. */
  @action public async fetchCallMeBackData(): Promise<void> {
    this.isLoading = true

    try {
      const response = await fetch(parseUrl(this.responseUrl, this.globalQueryParams), {
        headers: { Accept: 'application/json' },
      })

      // handle 5xx statuses and custom error message
      if (response.status.toString().startsWith('5')) {
        this.error = true
        const { message } = await (response.json() as Promise<CallMeBackErrorResponse>)
        this.errorMessage = message
        throw new Error(message)
      }

      // the rest of the statuses are treated as unknown errors
      if (!response.ok) {
        throw new Error(`FAILED TO FETCH  - ${response.status}`)
      }

      this.getResponseDetails = await (response.json() as Promise<IGetCallMeBackResponse>)
    } catch (error) {
      this.error = true
      // eslint-disable-next-line no-console
      console.error(String(error))
    } finally {
      this.isLoading = false
    }
  }

  /* POST data from form. */
  @action public async postCallMeBackData(
    data: ICallMeBackForm,
    url: string,
  ): Promise<IPostCallMeBackResponse | boolean> {
    this.isLoading = true

    try {
      this.isSuccess = true

      const response = await fetch(parseUrl(url, this.globalQueryParams), {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
      })

      if (!response.ok) {
        const { message } = await (response.json() as Promise<CallMeBackErrorResponse>)
        this.errorMessage = message
        throw new Error(`FAILED TO CONFIRM FORM - ${response.statusText}`)
      }

      return (this.postResponseDetails = await (response.json() as Promise<
        IPostCallMeBackResponse
      >))
    } catch (error) {
      this.error = true
      this.isSuccess = false
      // eslint-disable-next-line no-console
      console.error(error.toString())
      return false
    } finally {
      this.isLoading = false
    }
  }
}
