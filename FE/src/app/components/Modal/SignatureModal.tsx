import React, { useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react'
import classNames from 'classnames'
import { Alert, Button, Modal } from 'react-bootstrap'
import SignaturePad from 'react-signature-pad-wrapper'
import { useLabels } from '@hooks'
import { OfferStoreContext } from '@context'
import { PreloadImage } from '@components'
import { observer } from 'mobx-react-lite'
import { OfferStore } from '@stores'
import { isMobileDevice, parseUrl } from '@utils'

type Props = {
  id: string
  show: boolean
  onClose: () => void
  labels: Record<string, any>
  thumbnailUrl: string
  signFileUrl: string
  guid: string
}

export const SignatureModal: React.FC<Props> = observer(
  ({ id, show, onClose, labels, thumbnailUrl, signFileUrl, guid }) => {
    const store = useContext(OfferStoreContext)
    const t = useLabels(labels)
    const signatureRef = useRef<SignaturePad>()
    const [hasSignature, setHasSignature] = useState(false)
    const [step, setStep] = useState(1)

    if (!(store instanceof OfferStore)) {
      return null
    }

    // construct an URL for image preview of the document
    const documentUrl = useMemo(() => {
      const time = new Date().getTime()
      return `${thumbnailUrl}/${id}?t=${time}`
    }, [id, thumbnailUrl])

    // steps are relevant only for mobile devices
    const isFirstStep = useMemo(() => step === 1 && isMobileDevice(), [step])
    const showSignaturePad = useMemo(() => (step === 2 && isMobileDevice()) || !isMobileDevice(), [
      step,
    ])

    // 0. when the modal is shown => trigger `resize` event on window,
    // so signature canvas will fit into the parent container
    useEffect(() => {
      if (show && showSignaturePad) {
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
    }, [show, t, showSignaturePad])

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
      onClose() // callback from parent component
      setHasSignature(false) // next opening starts with an empty signature
      setStep(1) // clear the current progress
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
      const signed = await store.signDocument(id, signatureData, signFileUrl)

      // if request was successful => close the modal, otherwise display error
      if (signed) {
        closeModal()
      }
    }, [id, signFileUrl, store, closeModal])

    return (
      <Modal size="lg" show={show} onHide={closeModal} dialogClassName="modal-signature">
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

            {show && (isFirstStep || !isMobileDevice()) && (
              <>
                {/* Add tabindex on scrollable element causes the element scrollable by a keyboard. */}
                {/* eslint-disable-next-line jsx-a11y/no-noninteractive-tabindex */}
                <div className="document-wrapper mb-3" tabIndex={0}>
                  <PreloadImage
                    src={parseUrl(documentUrl, { guid })}
                    className="img-fluid d-block mx-auto"
                    alt={t('signatureModalThumbnailAlt')}
                  />
                </div>
              </>
            )}

            {showSignaturePad && (
              <>
                <p>{t('signatureModalText')}</p>
                <div className="signature mb-1">
                  <SignaturePad
                    height={isMobileDevice() ? 180 : 140}
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
              </>
            )}
          </Modal.Body>
          <Modal.Footer>
            {/* within the first mobile step show just `Sign` button */}
            {isFirstStep && (
              <Button variant="secondary" onClick={() => setStep(2)}>
                {t('signatureModalSign')}
              </Button>
            )}
            {/* within the second mobile step or on desktop, show the `Confirm` button */}
            {showSignaturePad && (
              <Button
                variant="primary"
                onClick={handleSubmit}
                disabled={store.isSigning || !hasSignature}
              >
                {t('signatureModalConfirm')}
              </Button>
            )}
            {isFirstStep && (
              <Button variant="dark" onClick={closeModal}>
                {t('signatureModalClose')}
              </Button>
            )}
            {showSignaturePad && (
              <Button variant="dark" onClick={handleClear}>
                {t('signatureModalClear')}
              </Button>
            )}
          </Modal.Footer>
        </div>
      </Modal>
    )
  },
)
