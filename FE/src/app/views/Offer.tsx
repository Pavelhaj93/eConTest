import React, { useEffect, useState, Fragment, useCallback } from 'react'
import { View } from '@types'
import { observer } from 'mobx-react-lite'
import { OfferStore } from '@stores'
import { Alert, Button, Form } from 'react-bootstrap'
import classNames from 'classnames'
import {
  Box,
  BoxHeading,
  FileDropZone,
  FileUpload,
  FormCheckWrapper,
  Icon,
  SignatureModal,
} from '@components'
import { colors } from '@theme'
import { useLabels } from '@hooks'
import { OfferStoreContext } from '@context'

type SignatureModalType = {
  id: string
  show: boolean
}

export const Offer: React.FC<View> = observer(
  ({
    doxReadyUrl,
    isRetention,
    isAcquisition,
    labels,
    formAction,
    getFileUrl,
    getFileForSignUrl,
    signFileUrl,
    doxTimeout,
    uploadFileUrl,
    removeFileUrl,
    errorPageUrl,
  }) => {
    const [store] = useState(() => new OfferStore(doxReadyUrl, isRetention, isAcquisition))
    const [signatureModalProps, setSignatureModalProps] = useState<SignatureModalType>({
      id: '',
      show: false,
    })
    const t = useLabels(labels)

    useEffect(() => {
      store.errorPageUrl = errorPageUrl
      store.fetchDocuments(doxTimeout)

      // set correct upload document URL if provided
      if (uploadFileUrl) {
        store.uploadDocumentUrl = uploadFileUrl
      }

      if (removeFileUrl) {
        store.removeDocumentUrl = removeFileUrl
      }
      // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    const openSignatureModal = useCallback((id: string) => {
      setSignatureModalProps({
        id,
        show: true,
      })
    }, [])

    return (
      <OfferStoreContext.Provider value={store}>
        <Fragment>
          {/* error state */}
          {store.error && (
            <Alert variant="danger">
              <h3>{t('appUnavailableTitle')}</h3>
              <div>{t('appUnavailableText')}</div>
            </Alert>
          )}
          {/* /error state */}

          <FileDropZone
            label={t('selectFileHelpText')}
            className="my-5"
            selectFileLabel={t('selectFile')}
            selectFileLabelMobile={t('uploadFile')}
            onFilesAccepted={files => {
              store.addUserFiles(files, 'category1')
            }}
            useCaptureOnMobile
            captureFileLabel={t('captureFile')}
          />

          {store.userDocuments['category1']?.length > 0 && (
            <Box>
              <ul aria-label={t('selectedFiles')} className="list-unstyled">
                {store.userDocuments['category1'].map(document => (
                  <li key={document.file.name}>
                    <FileUpload
                      file={document.file}
                      removeFileLabel={t('removeFile')}
                      onRemove={() => {
                        store.cancelUploadDocument(document)
                        store.removeUserDocument(document.file.name, 'category1')
                      }}
                      uploadHandler={() => store.uploadDocument(document, 'category1')}
                      // do not allow to reupload of already touched file (both with success or error)
                      shouldUploadImmediately={!document.touched}
                      error={document.error}
                      uploading={document.uploading}
                    />
                  </li>
                ))}
              </ul>
            </Box>
          )}

          <form
            action={formAction}
            method="post"
            className={classNames({
              'd-none': !store.isLoading && store.error, // hide the whole form if there is an error
            })}
          >
            {/* box with documents to be accepted */}
            <Box
              className={classNames({
                loading: store.isLoading,
              })}
            >
              {store.documentsToBeAccepted.length > 0 && (
                <Fragment>
                  <BoxHeading>Smlouva a přidružené dokumenty</BoxHeading>
                  <div className="mb-2">
                    <Button variant="link" onClick={() => store.acceptAllDocuments()}>
                      {t('acceptAll')}
                    </Button>
                  </div>
                  {store.documentsToBeAccepted.map(({ id, title, accepted, label }) => (
                    <FormCheckWrapper
                      key={id}
                      type="checkbox"
                      name="documents"
                      id={`document-${id}`}
                      value={id}
                      checked={accepted !== undefined ? accepted : false}
                      onChange={() => store.acceptDocument(id)}
                    >
                      <Form.Check.Label>
                        <span>
                          {label} <a href={`${getFileUrl}${id}`}>{title}</a>
                        </span>
                        <Icon name="pdf" size={36} color={colors.orange} className="ml-md-auto" />
                      </Form.Check.Label>
                    </FormCheckWrapper>
                  ))}
                </Fragment>
              )}

              {store.documentsToBeSigned.length > 0 && (
                <Fragment>
                  <BoxHeading>Dokumenty k podepsání</BoxHeading>
                  {store.documentsToBeSigned.map(({ id, title, label, signed }) => (
                    <div key={id} className="form-item-wrapper mb-3">
                      <div className="like-custom-control-label">
                        {signed && (
                          <Icon
                            name="check-circle"
                            size={36}
                            color={colors.green}
                            className="form-item-wrapper__icon mr-2"
                          />
                        )}
                        <span>
                          {label} <a href={`${getFileUrl}${id}`}>{title}</a>
                        </span>
                        {signed ? (
                          <Button
                            variant="primary"
                            className="btn-icon ml-auto form-item-wrapper__btn"
                            aria-label={t('signatureEditBtn')}
                            onClick={() => openSignatureModal(id)}
                          >
                            <Icon name="edit" size={18} color={colors.white} />
                          </Button>
                        ) : (
                          <Button
                            variant="secondary"
                            className="ml-auto form-item-wrapper__btn"
                            onClick={() => openSignatureModal(id)}
                          >
                            {t('signatureBtn')}
                          </Button>
                        )}
                      </div>
                    </div>
                  ))}
                  <p className="text-muted">
                    <small>{t('signatureModalHelpText')}</small>
                  </p>
                </Fragment>
              )}
            </Box>
            {/* /box with documents to be accepted */}

            {/**
             * submit zone
             * Hide the zone if there is an error or we didn't receive any documents.
             */}
            <div
              className={classNames({
                'd-none':
                  (!store.isLoading && store.error) ||
                  !store.documentsToBeAccepted.length ||
                  !store.documentsToBeSigned,
              })}
            >
              <h3>{t('acceptOfferTitle')}</h3>
              <Box>
                <p>{t('acceptOfferHelptext')}</p>
                <Button type="submit" variant="secondary" disabled={!store.isOfferReadyToAccept}>
                  {t('submitBtn')}
                </Button>
              </Box>
            </div>
            {/* submit zone */}
          </form>

          <SignatureModal
            {...signatureModalProps}
            onClose={() => setSignatureModalProps({ show: false, id: '' })}
            labels={labels}
            getFileForSignUrl={getFileForSignUrl ?? ''}
            signFileUrl={signFileUrl ?? ''}
          />
        </Fragment>
      </OfferStoreContext.Provider>
    )
  },
)
