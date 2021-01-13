import { observable } from 'mobx'
import { FileError } from '@types'

export class UserDocument {
  @observable
  public key: string

  @observable
  public file: File

  @observable
  public touched: boolean

  @observable
  public error: string | FileError | undefined

  @observable
  public uploading: boolean

  @observable
  public controller: AbortController | undefined

  constructor(
    file: File,
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
