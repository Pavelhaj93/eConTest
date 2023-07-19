import React, { useState } from 'react'
import { FileDropZone, FileUpload } from '@components'
import { useLabels } from '@hooks'
import { CustomFile } from '@types'

type Props = {
  labels: Record<string, any>
  allowedContentTypes?: string[]
  maxFileSize?: number
  onFilesAccepted?: (files: CustomFile[]) => void
  label?: string
  labelTooltip: string
  disabled?: boolean
}

export const UploadZone: React.FC<Props> = ({
  labels,
  allowedContentTypes,
  maxFileSize,
  onFilesAccepted,
  label,
  labelTooltip,
  disabled,
}) => {
  const [rejectedFiles, setRejectedFiles] = useState<CustomFile[]>([])
  const t = useLabels(labels)

  return (
    <>
      <FileDropZone
        label={label}
        labelTooltip={labelTooltip}
        accept={allowedContentTypes}
        maxFileSize={maxFileSize}
        helpText={t('selectFileHelpText')}
        selectFileLabel={t('selectFile')}
        selectFileLabelMobile={t('uploadFile')}
        // on each file change => clear the ones selected previously
        onFilesChanged={() => setRejectedFiles([])}
        onFilesAccepted={onFilesAccepted}
        onFilesRejected={files => setRejectedFiles(files)}
        useCaptureOnMobile
        captureFileLabel={t('captureFile')}
        disabled={disabled}
      />

      {/* custom rejected documents */}
      {rejectedFiles.length > 0 && (
        <ul aria-label={t('rejectedFiles')} className="list-unstyled border-bottom">
          {rejectedFiles.map((document, idx) => (
            <li key={`${idx}-${document.file.name}`} className="shake">
              <FileUpload
                file={document.file}
                labels={labels}
                shouldUploadImmediately={false}
                error={document.error}
                uploading={false}
              />
            </li>
          ))}
        </ul>
      )}
      {/* /custom rejected documents */}
    </>
  )
}
