import React, { useEffect, useState, Fragment, useCallback } from 'react'
import { View } from '@types'
import { observer } from 'mobx-react-lite'
import { OfferStore } from '@stores'
import { Alert, Button, Form } from 'react-bootstrap'
import classNames from 'classnames'
import { Box, BoxHeading, FileDropZone, FormCheckWrapper, Icon, SignatureModal } from '@components'
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
  }) => {
    const [store] = useState(() => new OfferStore(doxReadyUrl, isRetention, isAcquisition))
    const [signatureModalProps, setSignatureModalProps] = useState<SignatureModalType>({
      id: '',
      show: false,
    })
    const t = useLabels(labels)

    useEffect(() => {
      store.fetchDocuments(doxTimeout)
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
            onFilesAccepted={files => {
              console.log('accepted')
              console.log(files)
            }}
            onFilesRejected={files => {
              console.log('rejected')
              console.log(files)
            }}
          />

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
            getFileForSignUrl={getFileForSignUrl}
            signFileUrl={signFileUrl}
          />
        </Fragment>
      </OfferStoreContext.Provider>
    )
  },
)
