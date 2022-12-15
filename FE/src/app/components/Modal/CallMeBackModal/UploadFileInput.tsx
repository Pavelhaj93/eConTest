import React, { ChangeEvent, useCallback, useRef } from 'react'
import { Button, Form } from 'react-bootstrap'
import classNames from 'classnames'
import { generateId } from '@utils'

interface IUploadFileInput {
  file: File | undefined
  onChange: (event: ChangeEvent) => void
  onRemoveFile: () => void
  className?: string
  confirmationClassName?: string
  acceptedFormat: string
  labels: {
    insertFile: string
    removeFile: string
  }
}

export const UploadFileInput: React.FC<IUploadFileInput> = ({
  file,
  onRemoveFile,
  confirmationClassName,
  acceptedFormat,
  labels,
  onChange,
  className,
}): JSX.Element => {
  const inputIdRef = useRef<string>(`file_${generateId()}`)
  const inputClickRef = useRef<HTMLInputElement>(null)

  const openFileDialog = useCallback(() => {
    inputClickRef?.current?.click()
  }, [])

  return !file ? (
    <Form.Group className={classNames(className)}>
      <Button
        id={`${inputIdRef.current}`}
        className="cmb__label"
        variant="primary"
        onClick={openFileDialog}
      >
        {labels.insertFile}
      </Button>

      <Form.Control
        as="input"
        aria-labelledby={`${inputIdRef.current}`}
        className="d-none"
        type="file"
        accept={acceptedFormat}
        onChange={onChange}
        ref={inputClickRef}
      />
    </Form.Group>
  ) : (
    <div
      role="alert"
      className={classNames(
        'd-flex align-items-center justify-content-center font-weight-bold',
        confirmationClassName,
      )}
    >
      <p className="mr-4 mb-0">{file.name}</p>
      <Button
        aria-label={labels.removeFile}
        className="close cmb__additionalCloseBtn"
        variant="none"
        onClick={onRemoveFile}
      >
        <span aria-hidden="true">×</span>
      </Button>
    </div>
  )
}
