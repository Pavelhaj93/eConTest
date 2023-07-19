import { useKeepAlive, useLabels } from '@hooks'
import { View } from '@types'
import { parseUrl } from '@utils'
import { observer } from 'mobx-react-lite'
import React, { FC, Fragment, useEffect, useState } from 'react'
import { UploadStore } from '../stores/UploadStore'
import { Alert, Button } from 'react-bootstrap'
import classNames from 'classnames'
import { Box, FileUpload, UploadZone } from '@components'
import { computed } from 'mobx'

export const Upload: FC<View> = observer(
  ({
    uploadUrl,
    guid,
    labels,
    keepAliveUrl,
    timeout,
    allowedContentTypes,
    maxFileSize,
    uploadFileUrl,
    removeFileUrl,
    maxGroupFileSize,
    backUrl,
    nextUrl,
  }) => {
    const [store] = useState(() => new UploadStore(uploadUrl, guid))
    const t = useLabels(labels)

    // set correct upload document URL if provided
    if (uploadFileUrl) {
      store.uploadDocumentUrl = uploadFileUrl
    }

    if (removeFileUrl) {
      store.removeDocumentUrl = removeFileUrl
    }

    if (maxGroupFileSize) {
      store.maxUploadGroupSize = maxGroupFileSize
    }

    useEffect(() => {
      store.fetchUploads(timeout)
      // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    useKeepAlive(30 * 1000, keepAliveUrl ? parseUrl(keepAliveUrl, { guid }) : keepAliveUrl)

    return (
      <Fragment>
        {store.error && (
          <Alert variant="danger">
            <h3>{t('appUnavailableTitle')}</h3>
            <div
              className="editorial-content"
              dangerouslySetInnerHTML={{ __html: t('appUnavailableText') }}
            />
          </Alert>
        )}
        <div
          className={classNames({
            loading: store.isLoading,
            'd-none': store.error, // hide the whole box if there is an error
          })}
        >
          {store.uploadResponseItems?.map(item => (
            <Fragment key={item.position}>
              <h1 className="mt-5 text-center">{item.header?.title}</h1>
              <Box className="box__upload">
                <h2 className="text-center mb-4">{item.body.docs.title}</h2>
                {item.body.docs.files.map(({ id: categoryId, info, title }) => (
                  <div key={categoryId} className="mb-5">
                    <UploadZone
                      label={title}
                      labelTooltip={info ?? ''}
                      labels={labels}
                      allowedContentTypes={allowedContentTypes}
                      maxFileSize={maxFileSize}
                      onFilesAccepted={files => store.addUserFiles(files, categoryId)}
                      disabled={computed(() => store.uploadGroupSizeExceeded(item.position)).get()}
                    />
                    {/* uploaded documents by user*/}
                    {store.userDocuments[categoryId]?.length > 0 && (
                      <ul aria-label={t('selectedFiles')} className="list-unstyled">
                        {store.userDocuments[categoryId].map(document => (
                          <li key={document.key} className={classNames({ shake: document.error })}>
                            <FileUpload
                              file={document.file}
                              labels={labels}
                              onRemove={() => {
                                store.cancelUploadDocument(document)
                                store.removeUserDocument(document.key, categoryId)
                              }}
                              uploadHandler={() => store.uploadDocument(document, categoryId)}
                              // do not allow to reupload of already touched file (both with success or error)
                              shouldUploadImmediately={!document.touched}
                              error={document.error}
                              uploading={document.uploading}
                            />
                          </li>
                        ))}
                      </ul>
                    )}
                    {/*  uploaded documents by user */}
                    <div
                      className="text-left ml-1 text-muted"
                      dangerouslySetInnerHTML={{ __html: t('uploadFileRules') }}
                    />
                  </div>
                ))}
                <div
                  className="small text-muted editorial-content"
                  dangerouslySetInnerHTML={{ __html: item.body.docs.note }}
                />
              </Box>
            </Fragment>
          ))}
          <div className="text-center mb-3">
            {!store.isUploadFinished && (
              <div
                className="text-center mb-2"
                dangerouslySetInnerHTML={{ __html: t('beforeContinue') }}
              />
            )}
            <Button variant="primary" href={nextUrl} disabled={!store.isUploadFinished}>
              {t('continueBtn')}
            </Button>
          </div>
        </div>
        {backUrl && (
          <Button
            variant="link"
            className="underline text-primary m-auto w-content d-flex text-center"
            href={backUrl}
          >
            {t('backToSummary')}
          </Button>
        )}
      </Fragment>
    )
  },
)
