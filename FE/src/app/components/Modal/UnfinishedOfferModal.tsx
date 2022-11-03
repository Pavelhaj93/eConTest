import React, { useCallback, useContext } from 'react'
import { observer } from 'mobx-react-lite'
import { useLabels } from '@hooks'
import { Button, Modal } from 'react-bootstrap'
import { Icon } from '@components'
import { OfferStoreContext } from '@context'
import { OfferStore } from '@stores'
import classNames from 'classnames'

type Props = {
  show: boolean
  labels: Record<string, any>
  redirectUrl: string
  onClose: () => void
}

export const UnfinishedOfferModal: React.FC<Props> = observer(
  ({ show, labels, onClose, redirectUrl }) => {
    const t = useLabels(labels)
    const store = useContext(OfferStoreContext)

    if (!(store instanceof OfferStore)) {
      return null
    }

    const handleCancelOffer = useCallback(async () => {
      // cancel the offer
      const cancelled = await store?.cancelOffer()

      // if request was successful => redirect to the specified URL
      if (cancelled) {
        window.location.href = redirectUrl
      }
    }, [redirectUrl, store])

    return (
      <Modal size="xl" show={show} onHide={onClose}>
        <div className={classNames({ loading: store.isCancelling })}>
          <Modal.Header
            closeButton
            closeLabel={t('modalClose')}
            style={{
              borderBottom: '0 none',
            }}
          />

          <Modal.Body style={{ textAlign: 'center' }}>
            <Icon name="cross-circle" size={70} color="#eb4b0a"></Icon>
            <h2 className="text-red mt-4">{t('unfinishedOffer')}</h2>

            <div className="mb-4" dangerouslySetInnerHTML={{ __html: t('unfinishedOfferText') }} />

            <div className="d-flex flex-wrap justify-content-center" style={{ gap: '1.5rem' }}>
              <Button variant="primary" onClick={handleCancelOffer}>
                {t('quitOffer')}
              </Button>

              <Button variant="outline-danger" onClick={onClose}>
                {t('continueInOffer')}
              </Button>
            </div>
          </Modal.Body>
        </div>
      </Modal>
    )
  },
)
