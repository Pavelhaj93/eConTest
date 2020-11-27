import React from 'react'
import { useLabels } from '@hooks'
import { Button, Col, Modal, Row } from 'react-bootstrap'
import { Icon } from '@components'
import { colors } from '@theme'

type Props = {
  show: boolean
  labels: Record<string, any>
  onClose: () => void
  onConfirm: () => void
}

export const ConfirmationModal: React.FC<Props> = ({ show, labels, onClose, onConfirm }) => {
  const t = useLabels(labels)

  // TODO: make the modal body dynamic
  return (
    <Modal size="lg" show={show} onHide={onClose}>
      <Modal.Header closeButton>
        <Modal.Title>{t('confirmationModalTitle')}</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <p className="text-center">Prozatím statický obsah</p>
        <Row as="ul" className="justify-content-center list-unstyled mb-0 mt-3">
          <Col as="li" xs={6} lg={4} className="mb-4 text-center">
            <Icon name="check-circle" size={40} color={colors.green} />
            <span className="d-block mt-2 text-success">Smlouva / dodatek</span>
          </Col>
          <Col as="li" xs={6} lg={4} className="mb-4 text-center">
            <Icon name="check-circle" size={40} color={colors.green} />
            <span className="d-block mt-2 text-success">innogy Pojištění domácnosti</span>
          </Col>
          <Col as="li" xs={6} lg={4} className="mb-4 text-center">
            <Icon name="cross-circle" size={40} color={colors.red} />
            <span className="d-block mt-2 text-danger">Investor</span>
          </Col>
        </Row>
        <p className="text-center">{t('confirmationModalText')}</p>
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={onConfirm}>
          {t('confirmationModalAccept')}
        </Button>
        <Button variant="outline-dark" onClick={onClose}>
          {t('confirmationModalCancel')}
        </Button>
      </Modal.Footer>
    </Modal>
  )
}
