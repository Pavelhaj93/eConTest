import React, { useEffect, useMemo, useState } from 'react'
import classNames from 'classnames'
import { Button } from 'react-bootstrap'
import { Icon, IconName } from '@components'
import { colors } from '@theme'
import { formatBytes } from '@utils'
import { UploadDocumentResponse } from '@types'
import { useIsMountedRef } from '@hooks'

type Props = {
  file?: File
  /** Function that will be called when file is being removed. */
  onRemove?: () => void
  /** Set `aria-label` on remove button due to a11y. */
  removeFileLabel?: string
  /** Set a function responsible for uploading the file. */
  uploadHandler: (file: File) => Promise<UploadDocumentResponse>
  /** If set to `false`, the `uploadHandler` won't be called when component is mounted. */
  shouldUploadImmediately?: boolean
  /** Set error message. */
  error?: string | boolean
  uploading: boolean
}

export const FileUpload: React.FC<Props> = ({
  file,
  onRemove,
  removeFileLabel,
  uploadHandler,
  shouldUploadImmediately = true,
  error = false,
  uploading,
}) => {
  const [errorMessage, setErrorMessage] = useState<string | undefined | boolean>(error)
  const isMountedRef = useIsMountedRef()

  useEffect(() => {
    async function uploadFile(file: File) {
      const { message } = await uploadHandler(file)
      if (isMountedRef.current) {
        setErrorMessage(message)
      }
    }

    if (file && shouldUploadImmediately) {
      uploadFile(file)
    }
  }, [file, shouldUploadImmediately, uploadHandler, isMountedRef])

  const renderStatusIcon = useMemo(() => {
    let name: IconName = 'check-circle'
    let color = colors.green

    if (uploading) {
      name = 'refresh'
      color = colors.black
    } else if (errorMessage) {
      name = 'exclamation-mark-circle'
      color = colors.red
    }

    return (
      <Icon
        name={name}
        size={36}
        color={color}
        className={classNames({
          'mr-2': true,
          'spin-reverse': uploading,
        })}
      />
    )
  }, [uploading, errorMessage])

  if (!file) {
    return null
  }

  return (
    <div className="form-item-wrapper mb-3">
      <div className="like-custom-control-label">
        <div className="mr-2 d-inline-flex align-items-center flex-wrap">
          {renderStatusIcon}
          <div className={classNames({ 'text-danger': errorMessage })}>
            {file.name}
            {errorMessage && <small className="d-block">({errorMessage})</small>}
          </div>
        </div>
        <div className="ml-auto">
          {formatBytes(file.size)}
          {onRemove && (
            <Button
              variant="primary"
              className="btn-icon ml-2 form-item-wrapper__btn"
              aria-label={removeFileLabel && `${removeFileLabel} ${file.name}`}
              onClick={onRemove}
            >
              <Icon name="close" size={18} color={colors.white} />
            </Button>
          )}
        </div>
      </div>
    </div>
  )
}
