import React, { useEffect, useState, Fragment, useCallback } from 'react'
import { View } from '@types'
import { observer } from 'mobx-react-lite'
import { OfferStore } from '@stores'
import { Alert, Button, Form } from 'react-bootstrap'
import classNames from 'classnames'
import { Box, BoxHeading, FormCheckWrapper, Icon } from '@components'
import { colors } from '@theme'

export const Offer: React.FC<View> = observer(
  ({ doxReadyUrl, isRetention, isAcquisition, labels, formAction }) => {
    const [store] = useState(() => new OfferStore(doxReadyUrl, isRetention, isAcquisition))

    useEffect(() => {
      store.fetchDocuments()
      // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    const handleAcceptAll = useCallback(() => {
      store.acceptAllDocuments()
    }, [store])

    return (
      <Fragment>
        {/* error state */}
        {store.error && (
          <Alert variant="danger">
            <h3>{labels.appUnavailableTitle}</h3>
            <div>{labels.appUnavailableText}</div>
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
                  <Button variant="link" onClick={handleAcceptAll}>
                    {labels.acceptAll}
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
                {store.documentsToBeSigned.map(({ id, title, label }) => (
                  <div key={id}>
                    {label} {title}
                  </div>
                ))}
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
            <h3>Potvrzení nabídky</h3>
            <Box>
              <p>
                Stisknutím tlačítka Akceptuji potvrdíte souhlas s přechodem na novou smlouvu Relax u
                innogy.
              </p>
              <Button type="submit" variant="secondary" disabled={!store.isOfferReadyToAccept}>
                {labels.submitBtn}
              </Button>
            </Box>
          </div>
          {/* submit zone */}
        </form>
      </Fragment>
    )
  },
)
