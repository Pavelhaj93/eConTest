import React, { useCallback, useRef, useState, DragEvent, ChangeEvent } from 'react'
import classNames from 'classnames'
import { Button } from 'react-bootstrap'

type Props = {
  /** Text displayed in the middle of dropzone. */
  label?: string
  /** Label for main button. */
  selectFileLabel: string
  className?: string
  disabled?: boolean
  /** Set mime type restriction for the accepted files. */
  accept?: string[]
  /** Function that will be call whenever a new file is accepted. */
  onFilesAccepted: (files: File[]) => void
  /** Function that will be call whenever a new file is rejected. */
  onFilesRejected?: (files: File[]) => void
}

export const FileDropZone: React.FC<Props> = ({
  label,
  className,
  disabled = false,
  selectFileLabel,
  accept,
  onFilesAccepted,
  onFilesRejected,
}) => {
  const [active, setActive] = useState(false)
  const inputRef = useRef<HTMLInputElement>(null)

  const openFileDialog = useCallback(() => {
    if (disabled) return

    inputRef?.current?.click()
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
