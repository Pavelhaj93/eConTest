import React, { useCallback, useRef, useState, DragEvent, ChangeEvent, useMemo } from 'react'
import classNames from 'classnames'
import { Button } from 'react-bootstrap'
import { generateId, isMobileDevice } from '@utils'
import { Icon, Tooltip } from '@components'
import { colors } from '@theme'
import { FileError, CustomFile } from '@types'

type Props = {
  label?: string
  labelTooltip?: string
  /** Text displayed in the middle of dropzone. */
  helpText?: string
  /** Label for main button. */
  selectFileLabel: string
  /** Label for main button for mobile devices. */
  selectFileLabelMobile?: string
  captureFileLabel?: string
  className?: string
  disabled?: boolean
  /** Set mime type restriction for the accepted files. */
  accept?: string[]
  /** Set limit of file size. */
  maxFileSize?: number
  /** Function that will be called whenever a new file is accepted. */
  onFilesAccepted?: (files: CustomFile[]) => void
  /** Function that will be called whenever a new file is rejected. */
  onFilesRejected?: (files: CustomFile[]) => void
  /** Function that will be called whenever a new file is selected / dropped. */
  onFilesChanged?: (files: CustomFile[]) => void
  /** Set this to `true` if you want to render a different UI for mobile devices. */
  useCaptureOnMobile?: boolean
}

export const FileDropZone: React.FC<Props> = ({
  label,
  labelTooltip,
  helpText,
  className,
  disabled = false,
  selectFileLabel,
  accept,
  maxFileSize,
  onFilesAccepted,
  onFilesRejected,
  onFilesChanged,
  useCaptureOnMobile,
  selectFileLabelMobile,
  captureFileLabel,
}) => {
  const [active, setActive] = useState(false)
  const [isMobile] = useState(useCaptureOnMobile && isMobileDevice())
  const inputRef = useRef<HTMLInputElement>(null)
  const inputCaptureRef = useRef<HTMLInputElement>(null)
  const buttonId = useRef<string>(generateId())

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
      let allFiles: CustomFile[] = []
      let acceptedFiles: CustomFile[] = []
      let rejectedFiles: CustomFile[] = []

      files.forEach(file => {
        // check for mime type
        if (accept?.length && !accept.includes(file.type)) {
          rejectedFiles = [...rejectedFiles, { file, error: FileError.INVALID_TYPE }]
          allFiles = [...allFiles, { file, error: FileError.INVALID_TYPE }]

          // check for file size
        } else if (maxFileSize && file.size > maxFileSize) {
          rejectedFiles = [...rejectedFiles, { file, error: FileError.SIZE_EXCEEDED }]
          allFiles = [...allFiles, { file, error: FileError.SIZE_EXCEEDED }]

          // otherwise accept the file
        } else {
          acceptedFiles = [...acceptedFiles, { file }]
          allFiles = [...allFiles, { file }]
        }
      })

      onFilesChanged?.(allFiles)
      onFilesAccepted?.(acceptedFiles)
      onFilesRejected?.(rejectedFiles)
    },
    [accept, maxFileSize, onFilesAccepted, onFilesRejected, onFilesChanged],
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

  const renderLabel = useMemo(() => {
    if (label) {
      return (
        <div className="dropzone__label">
          <span
            className={classNames({
              'mr-2': Boolean(labelTooltip),
            })}
          >
            {label}
          </span>{' '}
          {labelTooltip && (
            <Tooltip size={20} iconColor="transparent" stroke={colors.orange}>
              {labelTooltip}
            </Tooltip>
          )}
        </div>
      )
    }
  }, [label, labelTooltip])

  if (isMobile) {
    return (
      <>
        {renderLabel}
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
              accept={accept?.join(',')}
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
      </>
    )
  }

  return (
    <>
      {renderLabel}
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
        {helpText && <p className="dropzone__helpText">{helpText}</p>}
        <Button
          id={`selectFile_${buttonId.current}`}
          variant="primary"
          onClick={openFileDialog}
          disabled={disabled}
          className="dropzone__button"
        >
          {selectFileLabel}
        </Button>
        <input
          type="file"
          ref={inputRef}
          className="dropzone__input"
          accept={accept?.join(',')}
          multiple
          autoComplete="off"
          tabIndex={0}
          disabled={disabled}
          onChange={handleFilesChange}
          aria-labelledby={`selectFile_${buttonId.current}`}
        />
      </div>
    </>
  )
}
