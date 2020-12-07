import React, { useCallback, useContext, useEffect, useRef } from 'react'
import classNames from 'classnames'
import { Alert, Button, Modal } from 'react-bootstrap'
import SignaturePad from 'react-signature-pad-wrapper'
import { useLabels } from '@hooks'
import { OfferStoreContext } from '@context'
import { PreloadImage } from '@components'
import { observer } from 'mobx-react-lite'
import { OfferStore } from '@stores'

type Props = {
  id: string
  show: boolean
  onClose: () => void
  labels: Record<string, any>
  thumbnailUrl: string
  signFileUrl: string
}

export const SignatureModal: React.FC<Props> = observer(
  ({ id, show, onClose, labels, thumbnailUrl, signFileUrl }) => {
    const store = useContext(OfferStoreContext)
    const t = useLabels(labels)
    const signatureRef = useRef<SignaturePad>()

    if (!(store instanceof OfferStore)) {
      return null
    }

    // 0. when the modal is shown => trigger `resize` event on window,
    // so signature canvas will fit into the parent container
    useEffect(() => {
      if (show) {
        const event = document.createEvent('HTMLEvents')
        event.initEvent('resize', true, false)
        window.dispatchEvent(event)

        // append some attributes for better a11y
        const canvas = signatureRef.current?.canvas
        if (canvas) {
          canvas.setAttribute('role', 'img')
          canvas.setAttribute('aria-label', t('signaturePadAlt'))
        }
      }
    }, [show, t])

    const handleClear = useCallback(() => {
      signatureRef.current?.clear()
    }, [])

    const handleSubmit = useCallback(async () => {
      const signature = signatureRef.current

      // do not continue if there is no signature
      if (!signature || signature.isEmpty()) {
        return
      }

      // get PNG as base64
      const signatureData = signature.toDataURL()

      // sign the document with user signature
      const signed = await store.signDocument(id, signatureData, signFileUrl)

      // if request was successful => close the modal, otherwise display error
      if (signed) {
        onClose()
      }
    }, [id, signFileUrl, store, onClose])

    return (
      <Modal size="lg" show={show} onHide={onClose}>
        <div className={classNames({ loading: store.isSigning })}>
          <Modal.Header closeButton closeLabel={t('modalClose')}>
            <Modal.Title>{t('signatureModalTitle')}</Modal.Title>
          </Modal.Header>
          <Modal.Body>
            {store.signError && (
              <Alert variant="danger" className="mt-0">
                {t('signatureModalError')}
              </Alert>
            )}
            {/* Add tabindex on scrollable element causes the element scrollable by a keyboard. */}
            {/* eslint-disable-next-line jsx-a11y/no-noninteractive-tabindex */}
            <div className="document-wrapper mb-3" tabIndex={0}>
              {show && (
                <PreloadImage
                  src={`${thumbnailUrl}/${id}`}
                  className="img-fluid d-block mx-auto"
                  alt={t('signatureModalThumbnailAlt')}
                />
              )}
            </div>
            <p>{t('signatureModalText')}</p>
            <div className="signature mb-2">
              <SignaturePad height={140} ref={signatureRef} />
            </div>
            <p className="text-muted mb-2">
              <small>{t('signatureModalHelpText')}</small>
            </p>
          </Modal.Body>
          <Modal.Footer>
            <Button variant="secondary" onClick={handleSubmit} disabled={store.isSigning}>
              {t('signatureModalConfirm')}
            </Button>
            <Button variant="dark" onClick={handleClear}>
              {t('signatureModalClear')}
            </Button>
          </Modal.Footer>
        </div>
      </Modal>
    )
  },
)
