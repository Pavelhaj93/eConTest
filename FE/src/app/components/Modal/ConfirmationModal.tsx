import React, { useContext } from 'react'
import { useLabels } from '@hooks'
import { Button, Col, Modal, Row } from 'react-bootstrap'
import classNames from 'classnames'
import { Icon } from '@components'
import { colors } from '@theme'
import { OfferStoreContext } from '@context'

type Props = {
  show: boolean
  labels: Record<string, any>
  onClose: () => void
  onConfirm: () => void
}

export const ConfirmationModal: React.FC<Props> = ({ show, labels, onClose, onConfirm }) => {
  const store = useContext(OfferStoreContext)
  const t = useLabels(labels)

  return (
    <Modal size="lg" show={show} onHide={onClose}>
      <Modal.Header closeButton>
        <Modal.Title>{t('acceptanceModalTitle')}</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Row as="ul" className="justify-content-center list-unstyled mb-0 mt-3">
          {store?.acceptance.params.map(({ group, title, accepted }) => (
            <Col key={group} as="li" xs={6} lg={4} className="mb-4 text-center">
              <Icon name="check-circle" size={40} color={accepted ? colors.green : colors.red} />
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
        <Button variant="secondary" onClick={onConfirm}>
          {t('acceptanceModalAccept')}
        </Button>
        <Button variant="outline-dark" onClick={onClose}>
          {t('acceptanceModalCancel')}
        </Button>
      </Modal.Footer>
    </Modal>
  )
}
