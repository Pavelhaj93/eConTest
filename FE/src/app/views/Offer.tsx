import React, { useEffect, useState, Fragment, useCallback } from 'react'
import { View } from '@types'
import { observer } from 'mobx-react-lite'
import { OfferStore } from '@stores'
import { Alert, Button, Form } from 'react-bootstrap'
import classNames from 'classnames'
import { Box, BoxHeading, FormCheckWrapper, Icon, SignatureModal } from '@components'
import { colors } from '@theme'
import { useLabels } from '@hooks'

export const Offer: React.FC<View> = observer(
  ({ doxReadyUrl, isRetention, isAcquisition, labels, formAction }) => {
    const [store] = useState(() => new OfferStore(doxReadyUrl, isRetention, isAcquisition))
    const [signatureModalShow, setSignatureModalShow] = useState(false)
    const t = useLabels(labels)

    useEffect(() => {
      store.fetchDocuments()
      // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    const openSignatureModal = useCallback((id: string) => {
      setSignatureModalShow(true)
    }, [])

    return (
      <Fragment>
        {/* error state */}
        {store.error && (
          <Alert variant="danger">
            <h3>{t('appUnavailableTitle')}</h3>
            <div>{t('appUnavailableText')}</div>
          </Alert>
        )}
        {/* /error state */}

        {/* TODO: maybe hide the whole form in case of error */}
        <form action={formAction} method="post">
          {/* box with documents to be accepted */}
          <Box
            className={classNames({
              loading: store.isLoading,
              'd-none': !store.isLoading && store.error, // hide the whole box if there is an error
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
                {store.documentsToBeAccepted.map(({ id, title, accepted, url, label }) => (
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
                        {label} <a href={url}>{title}</a>
                      </span>
                      <Icon name="pdf" size={36} color={colors.orange} className="ml-md-auto" />
                    </Form.Check.Label>
                  </FormCheckWrapper>
                ))}
              </Fragment>
            )}

            {/* TODO: create signature functionality */}
            {store.documentsToBeSigned.length > 0 && (
              <Fragment>
                <BoxHeading>Dokumenty k podepsání</BoxHeading>
                {store.documentsToBeSigned.map(({ id, title, label, url }) => (
                  <div key={id} className="form-check-wrapper mb-3">
                    <div className="like-custom-control-label">
                      <span>
                        {label} <a href={url}>{title}</a>
                      </span>
                      <Button
                        variant="secondary"
                        className="ml-auto"
                        onClick={() => openSignatureModal(id)}
                      >
                        {t('signatureBtn')}
                      </Button>
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
          show={signatureModalShow}
          onClose={() => setSignatureModalShow(false)}
          labels={labels}
        />
      </Fragment>
    )
  },
)
