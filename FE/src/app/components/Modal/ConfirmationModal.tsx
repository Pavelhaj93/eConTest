import React, { useCallback, useContext, useState } from 'react'
import { observer } from 'mobx-react-lite'
import { useLabels } from '@hooks'
import { Alert, Button, Col, Modal, Row } from 'react-bootstrap'
import classNames from 'classnames'
import { Icon } from '@components'
import { colors } from '@theme'
import { OfferStoreContext } from '@context'
import { OfferStore } from '@stores'

type Props = {
  show: boolean
  labels: Record<string, any>
  onClose: () => void
  thankYouPageUrl: string
}

export const ConfirmationModal: React.FC<Props> = observer(
  ({ show, labels, onClose, thankYouPageUrl }) => {
    const [error, setError] = useState(false)
    const [isRedirecting, setRedirecting] = useState(false)
    const t = useLabels(labels)
    const store = useContext(OfferStoreContext)

    if (!(store instanceof OfferStore)) {
      return null
    }

    const handleAcceptOffer = useCallback(async () => {
      if (!window.dataLayer) {
        window.dataLayer = []
      }
      window.dataLayer.push({
        event: 'gaEvent',
        gaEventData: {
          eCat: 'eContracting',
          eAct: 'Offer accepted',
        },
        eventCallback: function () {
          window.dataLayer.push({ gaEventData: undefined })
        },
      })

      const accepted = await store.acceptOffer()

      if (accepted) {
        // keep the loading spinner and disabled buttons even during the redirection
        setRedirecting(true)
        window.location.href = thankYouPageUrl
      } else {
        setError(true)
      }
    }, [store, thankYouPageUrl])

    return (
      <Modal size="lg" show={show} onHide={onClose}>
        <Modal.Header closeButton>
          <Modal.Title>{t('acceptanceModalTitle')}</Modal.Title>
        </Modal.Header>
        <Modal.Body
          className={classNames({
            loading: store.isAccepting || isRedirecting,
          })}
        >
          {error && (
            <Alert variant="danger" className="mt-0">
              {t('acceptanceModalError')}
            </Alert>
          )}
          <Row as="ul" className="justify-content-center list-unstyled mb-0 mt-3">
            {store.acceptanceGroups.map(({ group, title, accepted }) => (
              <Col key={group} as="li" xs={6} lg={4} className="mb-4 text-center">
                <Icon
                  name={accepted ? 'check-circle' : 'cross-circle'}
                  size={40}
                  color={accepted ? colors.green : colors.red}
                />
                <span
                  className={classNames({
                    'd-block': true,
                    'mt-2': true,
                    'text-success': accepted,
                    'text-danger': !accepted,
                  })}
                >
                  {title}
                </span>
              </Col>
            ))}
          </Row>
          <div
            className="text-center editorial-content"
            dangerouslySetInnerHTML={{ __html: t('acceptanceModalText') }}
          />
        </Modal.Body>
        <Modal.Footer>
          <Button
            variant="secondary"
            onClick={handleAcceptOffer}
            disabled={store.isAccepting || isRedirecting}
          >
            {t('acceptanceModalAccept')}
          </Button>
          <Button
            variant="outline-dark"
            onClick={onClose}
            disabled={store.isAccepting || isRedirecting}
          >
            {t('acceptanceModalCancel')}
          </Button>
        </Modal.Footer>
      </Modal>
    )
  },
)
