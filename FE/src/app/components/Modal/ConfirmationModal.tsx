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
  openUnfinishedModal?: () => void
  thankYouPageUrl: string
  cancelDialog?: boolean
}

export const ConfirmationModal: React.FC<Props> = observer(
  ({ show, labels, onClose, thankYouPageUrl, cancelDialog, openUnfinishedModal }) => {
    const [error, setError] = useState(false)
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
        window.location.href = thankYouPageUrl
      } else {
        setError(true)
      }
    }, [store, thankYouPageUrl])

    return (
      <Modal size="lg" show={show} onHide={onClose}>
        <Modal.Header closeButton closeLabel={t('modalClose')}>
          <Modal.Title>{t('acceptanceModalTitle')}</Modal.Title>
        </Modal.Header>
        <Modal.Body
          className={classNames({
            loading: store.isAccepting,
          })}
        >
          {error && (
            <Alert variant="danger" className="mt-0">
              {t('acceptanceModalError')}
            </Alert>
          )}
          <Row
            as="ul"
            className="justify-content-center list-unstyled mb-0 mt-3"
            aria-label={t('acceptanceModalSummary')}
          >
            {store.acceptanceGroups?.map(({ group, title, accepted }) => (
              <Col key={group} as="li" xs={6} lg={4} className="mb-4 text-center">
                <Icon
                  name={accepted ? 'check-circle' : 'cross-circle'}
                  size={40}
                  color={accepted ? colors.green : colors.red}
                />
                <span className="sr-only">
                  {title} {accepted ? t('accepted') : t('notAccepted')}
                </span>
                <span
                  className={classNames({
                    'd-block': true,
                    'mt-2': true,
                    'text-success': accepted,
                    'text-danger': !accepted,
                  })}
                  aria-hidden="true"
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
        <Modal.Footer className="flex-column">
          <div>
            <Button variant="secondary" onClick={handleAcceptOffer} disabled={store.isAccepting}>
              {t('acceptanceModalAccept')}
            </Button>
            <Button
              className="ml-3"
              variant="outline-dark"
              onClick={onClose}
              disabled={store.isAccepting}
            >
              {t('acceptanceModalCancel')}
            </Button>
          </div>
          {/* If a user wants to start again, the following button will be visible */}
          {cancelDialog && openUnfinishedModal && (
            <Button variant="link" type="submit" onClick={openUnfinishedModal}>
              {t('cancelOffer')}
            </Button>
          )}
        </Modal.Footer>
      </Modal>
    )
  },
)
