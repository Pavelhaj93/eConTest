import { observable } from 'mobx'
import { FileError, StoredUploadFile } from '@types'

export class UserDocument {
  @observable
  public key: string

  @observable
  public file: File | StoredUploadFile

  @observable
  public touched: boolean

  @observable
  public error: string | FileError | undefined

  @observable
  public uploading: boolean

  @observable
  public controller: AbortController | undefined

  constructor(
    file: File | StoredUploadFile,
    key: string,
    touched?: boolean,
    error?: string,
    uploading?: boolean,
    controller?: AbortController,
  ) {
    this.file = file
    this.touched = Boolean(touched)
    this.error = error
    this.uploading = Boolean(uploading)
    this.controller = controller
    this.key = key
  }
}
