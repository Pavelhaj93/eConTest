import React, { useCallback, useRef, useState, DragEvent, ChangeEvent } from 'react'
import classNames from 'classnames'
import { Button } from 'react-bootstrap'
import { isMobileDevice } from '@utils'
import { Icon } from '@components'
import { colors } from '@theme'

type Props = {
  /** Text displayed in the middle of dropzone. */
  label?: string
  /** Label for main button. */
  selectFileLabel: string
  /** Label for main button for mobile devices. */
  selectFileLabelMobile?: string
  captureFileLabel?: string
  className?: string
  disabled?: boolean
  /** Set mime type restriction for the accepted files. */
  accept?: string[]
  /** Function that will be call whenever a new file is accepted. */
  onFilesAccepted: (files: File[]) => void
  /** Function that will be call whenever a new file is rejected. */
  onFilesRejected?: (files: File[]) => void
  /** Set this to `true` if you want to render a different UI for mobile devices. */
  useCaptureOnMobile?: boolean
}

export const FileDropZone: React.FC<Props> = ({
  label,
  className,
  disabled = false,
  selectFileLabel,
  accept,
  onFilesAccepted,
  onFilesRejected,
  useCaptureOnMobile,
  selectFileLabelMobile,
  captureFileLabel,
}) => {
  const [active, setActive] = useState(false)
  const [isMobile] = useState(useCaptureOnMobile && isMobileDevice())
  const inputRef = useRef<HTMLInputElement>(null)
  const inputCaptureRef = useRef<HTMLInputElement>(null)

  const openFileDialog = useCallback(() => {
    if (disabled) return

    inputRef?.current?.click()
  }, [disabled])

  const openDeviceCamera = useCallback(() => {
    if (disabled) return

    inputCaptureRef?.current?.click()
  }, [disabled])

  const processFiles = useCallback(
    (files: File[]) => {
      let acceptedFiles: File[] = files
      let rejectedFiles: File[] = []

      // check for mime types
      if (accept?.length) {
        // start with an empty array
        acceptedFiles = []

        files.forEach(file => {
          if (accept.includes(file.type)) {
            acceptedFiles = [...acceptedFiles, file]
          } else {
            rejectedFiles = [...rejectedFiles, file]
          }
        })
      }

      onFilesAccepted(acceptedFiles)
      onFilesRejected && onFilesRejected(rejectedFiles)
    },
    [accept, onFilesAccepted, onFilesRejected],
  )

  const handleDragOver = useCallback(
    (event: DragEvent) => {
      event.preventDefault()

      if (disabled) return

      setActive(true)
    },
    [disabled],
  )

  const handleDragLeave = useCallback((event: DragEvent) => {
    event.preventDefault()
    setActive(false)
  }, [])

  const handleDrop = useCallback(
    (event: DragEvent) => {
      event.preventDefault()

      if (disabled) return

      // convert FileList to a plain array
      const files = Array.from(event.dataTransfer.files)
      processFiles(files)

      setActive(false)
    },
    [disabled, processFiles],
  )

  // need prevent default `dragEnter` event as well (IE 11)
  const handleDragEnter = useCallback((event: DragEvent) => {
    event.preventDefault()
  }, [])

  const handleFilesChange = useCallback(
    (event: ChangeEvent<HTMLInputElement>) => {
      // convert FileList to a plain array
      const files = Array.from(event.target.files ?? [])
      processFiles(files)
    },
    [processFiles],
  )

  if (isMobile) {
    return (
      <div
        className={classNames({
          'dropzone--mobile': true,
          [className ?? '']: className,
          'dropzone--disabled': disabled,
        })}
      >
        {/* standard file selection */}
        <div className="mb-2">
          <Button
            variant="primary"
            onClick={openFileDialog}
            disabled={disabled}
            className="btn-block-mobile"
          >
            {selectFileLabelMobile}
          </Button>
          <input
            type="file"
            ref={inputRef}
            className="dropzone__input"
            multiple
            autoComplete="off"
            tabIndex={0}
            disabled={disabled}
            onChange={handleFilesChange}
          />
        </div>

        {/* open device camera directly */}
        <Button
          variant="link"
          onClick={openDeviceCamera}
          disabled={disabled}
          className="d-inline-flex align-items-center"
        >
          <Icon
            name="photo-circle"
            size={36}
            color={disabled ? colors.gray40 : colors.orange}
            className="mr-2"
          />
          {captureFileLabel}
        </Button>
        <input
          type="file"
          accept="image/*"
          capture="environment"
          ref={inputCaptureRef}
          className="dropzone__input"
          multiple
          autoComplete="off"
          tabIndex={0}
          disabled={disabled}
          onChange={handleFilesChange}
        />
      </div>
    )
  }

  return (
    <div
      className={classNames({
        dropzone: true,
        [className ?? '']: className,
        'dropzone--disabled': disabled,
        'dropzone--active': active,
      })}
      onDragOver={handleDragOver}
      onDragLeave={handleDragLeave}
      onDrop={handleDrop}
      onDragEnter={handleDragEnter}
    >
      {label && <p>{label}</p>}
      <Button variant="primary" onClick={openFileDialog} disabled={disabled}>
        {selectFileLabel}
      </Button>
      <input
        type="file"
        ref={inputRef}
        className="dropzone__input"
        multiple
        autoComplete="off"
        tabIndex={0}
        disabled={disabled}
        onChange={handleFilesChange}
      />
    </div>
  )
}
