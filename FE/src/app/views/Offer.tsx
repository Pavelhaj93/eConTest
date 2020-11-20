import React, { useEffect, useState, Fragment, useCallback, useRef } from 'react'
import { OfferType, View } from '@types'
import { observer } from 'mobx-react-lite'
import { OfferStore } from '@stores'
import { Alert, Button, Form } from 'react-bootstrap'
import classNames from 'classnames'
import {
  Box,
  BoxHeading,
  ConfirmationModal,
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
    offerUrl,
    labels,
    formAction,
    getFileUrl,
    getFileForSignUrl,
    signFileUrl,
    doxTimeout,
    uploadFileUrl,
    removeFileUrl,
    errorPageUrl,
    allowedContentTypes,
    maxFileSize,
  }) => {
    const [store] = useState(() => new OfferStore(OfferType.NEW, offerUrl))
    const [signatureModalProps, setSignatureModalProps] = useState<SignatureModalType>({
      id: '',
      show: false,
    })
    const [confirmationModal, setConfirmationModal] = useState(false)
    const t = useLabels(labels)
    const formRef = useRef<HTMLFormElement>(null)

    useEffect(() => {
      store.errorPageUrl = errorPageUrl
      // store.fetchOffer(doxTimeout)

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

          <Box>
            <FileDropZone
              accept={allowedContentTypes}
              maxFileSize={maxFileSize}
              label={t('selectFileHelpText')}
              className="my-5"
              selectFileLabel={t('selectFile')}
              selectFileLabelMobile={t('uploadFile')}
              onFilesChanged={files => store.addUserFiles(files, 'category1')}
              useCaptureOnMobile
              captureFileLabel={t('captureFile')}
            />
            {store.userDocuments['category1']?.length > 0 && (
              <ul aria-label={t('selectedFiles')} className="list-unstyled">
                {store.userDocuments['category1'].map(document => (
                  <li key={document.id}>
                    <FileUpload
                      file={document.file}
                      labels={labels}
                      onRemove={() => {
                        store.cancelUploadDocument(document)
                        store.removeUserDocument(document.id, 'category1')
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
            )}
          </Box>

          <form
            action={formAction}
            method="post"
            ref={formRef}
            className={classNames({
              'd-none': !store.isLoading && store.error, // hide the whole form if there is an error
            })}
          >
            {/* box with documents to be accepted */}
            {/* <Box
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
                  {store.documentsToBeAccepted.map(({ key, title, accepted, label }) => (
                    <FormCheckWrapper
                      key={key}
                      type="checkbox"
                      name="documents"
                      id={`document-${key}`}
                      value={key}
                      checked={accepted !== undefined ? accepted : false}
                      onChange={() => store.acceptDocument(key)}
                    >
                      <Form.Check.Label>
                        <span>
                          {label} <a href={`${getFileUrl}${key}`}>{title}</a>
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
                  {store.documentsToBeSigned.map(({ key, title, label, signed }) => (
                    <div key={key} className="form-item-wrapper mb-3">
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
                          {label} <a href={`${getFileUrl}${key}`}>{title}</a>
                        </span>
                        {signed ? (
                          <Button
                            variant="primary"
                            className="btn-icon ml-auto form-item-wrapper__btn"
                            aria-label={t('signatureEditBtn')}
                            onClick={() => openSignatureModal(key)}
                          >
                            <Icon name="edit" size={18} color={colors.white} />
                          </Button>
                        ) : (
                          <Button
                            variant="secondary"
                            className="ml-auto form-item-wrapper__btn"
                            onClick={() => openSignatureModal(key)}
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
            </Box> */}
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
                <Button
                  variant="secondary"
                  onClick={() => setConfirmationModal(true)}
                  disabled={!store.isOfferReadyToAccept}
                >
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

          <ConfirmationModal
            show={confirmationModal}
            onClose={() => setConfirmationModal(false)}
            // TODO: add gaEvent (dataLayer.push)
            onConfirm={() => formRef.current?.submit()}
            labels={labels}
          />
        </Fragment>
      </OfferStoreContext.Provider>
    )
  },
)
