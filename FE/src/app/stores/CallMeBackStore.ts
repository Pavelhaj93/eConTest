import { action, observable } from 'mobx'
import {
  CallMeBackErrorResponse,
  ICallMeBackForm,
  IPostCallMeBackResponse,
  FileVariant,
  IGetCallMeBackResponse,
} from '../types/CallMeBack'
import { isGetResponseSucceed, parseUrl } from '@utils'
import { QueryParams } from '@types'
import { ChangeEvent } from 'react'

export class CallMeBackStore {
  private globalQueryParams: QueryParams

  @observable
  public phoneValue = ''

  @observable
  public timeValue = ''

  @observable
  public noteValue = ''

  @observable
  public fileA: File | undefined = undefined

  @observable
  public fileB: File | undefined = undefined

  @observable
  public isFileSizeError = false

  @observable
  public isNoteOpen = false

  @observable
  public isFileOpen = false

  @observable
  public showConfirmationModal = false

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

  @action public setError(error: boolean): void {
    this.error = error
  }

  @action public setNoteValue(noteValue: string): void {
    this.noteValue = noteValue
  }

  @action public setPhoneValue(phoneValue: string): void {
    this.phoneValue = phoneValue
  }

  @action public setTimeValue(timeValue: string): void {
    this.timeValue = timeValue
  }

  @action public setFileA(fileA: File | undefined): void {
    this.fileA = fileA
  }

  @action public setFileB(fileB: File | undefined): void {
    this.fileB = fileB
  }

  @action public setFileSizeError(isFileSizeError: boolean): void {
    this.isFileSizeError = isFileSizeError
  }

  @action public setIsNoteOpen(isNoteOpen: boolean): void {
    this.isNoteOpen = isNoteOpen
  }

  @action public setIsFileOpen(isFileOpen: boolean): void {
    this.isFileOpen = isFileOpen
  }

  @action public setShowConfirmationModal(showConfirmationModal: boolean): void {
    this.showConfirmationModal = showConfirmationModal
  }

  @action public setErrorMessage(errorMessage: string): void {
    this.errorMessage = errorMessage
  }

  /* Handle close modal, store value. */
  @action public handleCloseModalStoreValue(): void {
    this.setError(false)
    this.setIsNoteOpen(false)
    this.setIsFileOpen(false)
    this.setFileA(undefined)
    this.setFileB(undefined)
    this.setShowConfirmationModal(false)
    this.setFileSizeError(false)
    this.setNoteValue('')
    this.setTimeValue('')

    if (isGetResponseSucceed(this.getResponseDetails)) {
      this.setPhoneValue(this.getResponseDetails?.phone || '')
    }
  }

  /* Set file handler. */
  @action public handleSetFile = (event: ChangeEvent, fileVariant: FileVariant): void => {
    const target = event.target as HTMLInputElement

    if (!target.files) {
      return
    }

    const fileSizeExceeded =
      target.files[0].size >
      (isGetResponseSucceed(this.getResponseDetails)
        ? this.getResponseDetails.maxFileSize
        : 2000000)

    if (fileSizeExceeded) {
      fileVariant === 'A' ? this.setFileA(undefined) : this.setFileB(undefined)
      this.setFileSizeError(true)
    } else {
      fileVariant === 'A' ? this.setFileA(target.files[0]) : this.setFileB(target.files[0])
      this.setFileSizeError(false)
    }
  }

  /* Handle custom simulated 5xx response as 200 error response. */
  @action public setCustom500Error = (res: IGetCallMeBackResponse): void => {
    if (!isGetResponseSucceed(res)) {
      this.setErrorMessage(res.error)
      this.setError(true)
    }
  }

  /* Get data for form. */
  @action public async fetchCallMeBackData(): Promise<void> {
    this.isLoading = true

    try {
      const response = await fetch(parseUrl(this.responseUrl, this.globalQueryParams), {
        headers: { Accept: 'application/json' },
      })

      // the rest of the statuses are treated as unknown errors
      if (!response.ok) {
        throw new Error(`FAILED TO FETCH  - ${response.status}`)
      }

      this.getResponseDetails = await (response.json() as Promise<IGetCallMeBackResponse>)

      /* Set custom simulated 5xx response as 200 error response. */
      this.setCustom500Error(this.getResponseDetails)
    } catch (error) {
      this.setError(true)
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

    const formData = new FormData()
    const { phone, time, note, file, currentBrowserUrl } = data

    formData.append('phone', phone)
    formData.append('time', time)
    formData.append('note', note)
    formData.append('currentBrowserUrl', currentBrowserUrl)

    if (file) {
      typeof file[0] !== 'undefined' && formData.append('file', file[0], file[0].name)
    }

    if (file) {
      typeof file[1] !== 'undefined' && formData.append('file', file[1], file[1].name)
    }

    try {
      this.isSuccess = true

      const response = await fetch(parseUrl(url, this.globalQueryParams), {
        method: 'POST',
        headers: { Accept: 'application/json' },
        body: formData,
      })

      if (!response.ok) {
        const { error } = await (response.json() as Promise<CallMeBackErrorResponse>)
        this.setErrorMessage(error)
        throw new Error(`FAILED TO CONFIRM FORM - ${response.statusText}`)
      }

      return (this.postResponseDetails = await (response.json() as Promise<
        IPostCallMeBackResponse
      >))
    } catch (error) {
      this.setError(true)
      this.isSuccess = false
      // eslint-disable-next-line no-console
      console.error(String(error))
      return false
    } finally {
      this.isLoading = false
    }
  }
}
