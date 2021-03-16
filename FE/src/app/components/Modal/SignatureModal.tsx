import React, { useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react'
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
    const [hasSignature, setHasSignature] = useState(false)

    if (!(store instanceof OfferStore)) {
      return null
    }

    // construct an URL for image preview of the document
    const documentUrl = useMemo(() => {
      const time = new Date().getTime()
      return `${thumbnailUrl}/${id}?t=${time}`
    }, [id, thumbnailUrl])

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
      setHasSignature(false)
    }, [])

    const handleDrawEnd = useCallback(() => {
      if (!signatureRef.current?.isEmpty()) {
        setHasSignature(true)
      }
    }, [])

    const closeModal = useCallback(() => {
      console.log('onClose', onClose)
      console.log('closing dialog')
      onClose() // callback from parent component
      setHasSignature(false) // next opening starts with an empty signature
    }, [onClose])

    const handleSubmit = useCallback(async () => {
      const signature = signatureRef.current

      // do not continue if there is no signature
      if (!signature || signature.isEmpty()) {
        return
      }

      // get PNG as base64
      const signatureData = signature.toDataURL()

      // sign the document with user signature
      console.log('before sign')
      const signed = await store.signDocument(id, signatureData, signFileUrl)
      console.log('after sign, signed = ', signed)

      // if request was successful => close the modal, otherwise display error
      if (signed) {
        closeModal()
      }
    }, [id, signFileUrl, store, closeModal])

    return (
      <Modal size="lg" show={show} onHide={closeModal}>
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
                  src={documentUrl}
                  className="img-fluid d-block mx-auto"
                  alt={t('signatureModalThumbnailAlt')}
                />
              )}
            </div>
            <p>{t('signatureModalText')}</p>
            <div className="signature mb-1">
              <SignaturePad
                height={140}
                ref={signatureRef}
                redrawOnResize={true}
                options={{ onEnd: handleDrawEnd }}
              />
            </div>
            <div
              className="editorial-content text-muted small mb-2"
              dangerouslySetInnerHTML={{
                __html: t('signatureNote'),
              }}
              aria-hidden="true"
            />
          </Modal.Body>
          <Modal.Footer>
            <Button
              variant="secondary"
              onClick={handleSubmit}
              disabled={store.isSigning || !hasSignature}
            >
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
