import { observable } from 'mobx'

export class UserDocument {
  @observable
  public file: File

  @observable
  public touched: boolean

  @observable
  public error: string | undefined

  @observable
  public uploading: boolean

  @observable
  public controller: AbortController | undefined

  constructor(
    file: File,
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
  }
}
