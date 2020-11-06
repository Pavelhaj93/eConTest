import React, { useCallback, useEffect, useRef } from 'react'
import { Button, Modal } from 'react-bootstrap'
import SignaturePad from 'react-signature-pad-wrapper'
import { useLabels } from '@hooks'

type Props = {
  show: boolean
  onClose: () => void
  labels: Record<string, any>
}

export const SignatureModal: React.FC<Props> = ({ show, onClose, labels }) => {
  const t = useLabels(labels)
  const signatureRef = useRef<SignaturePad>()

  // 0. when the modal is shown => trigger `resize` event on window,
  // so signature canvas will fit into the parent container
  useEffect(() => {
    if (show) {
      const event = document.createEvent('HTMLEvents')
      event.initEvent('resize', true, false)
      window.dispatchEvent(event)
    }
  }, [show])

  const handleClear = useCallback(() => {
    signatureRef.current?.clear()
  }, [])

  const handleSubmit = useCallback(() => {
    // const signatureData = signatureRef.current?.toDataURL()
    // console.log(signatureData)
  }, [])

  return (
    <Modal size="lg" show={show} onHide={onClose}>
      <Modal.Header closeButton>
        <Modal.Title>{t('signatureModalTitle')}</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <p>{t('signatureModalText')}</p>
        <div className="signature mb-2">
          <SignaturePad height={150} ref={signatureRef} />
        </div>
        <p className="text-muted mb-2">
          <small>{t('signatureModalHelpText')}</small>
        </p>
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={handleSubmit}>
          {t('signatureModalConfirm')}
        </Button>
        <Button variant="dark" onClick={handleClear}>
          {t('signatureModalClear')}
        </Button>
      </Modal.Footer>
    </Modal>
  )
}
