import React, { useContext, useState } from 'react'
import { observer } from 'mobx-react-lite'
import { Button, Col, Form, Modal, Row } from 'react-bootstrap'
import classNames from 'classnames'
import { colors } from '@theme'
import { useForm } from 'react-hook-form'
import { ICallMeBackForm } from '../../types/CallMeBack'
import { CallMeBackStoreContext } from '@context'
import { CallMeBackStore } from '@stores'
import { Icon } from '../Icon'
import { generateId } from '@utils'

type Props = {
  show: boolean
  onClose: () => void
  postCallMeBackUrl: string
}

/* ID for phone input. */
const phoneId = `phone_${generateId()}`
/* ID for select input. */
const timeId = `when_${generateId()}`

export const CallMeBackModal: React.FC<Props> = observer(({ show, onClose, postCallMeBackUrl }) => {
  const store = useContext(CallMeBackStoreContext)

  if (!(store instanceof CallMeBackStore)) {
    return null
  }

  const [noteOpen, setNoteOpen] = useState<boolean>(false)
  const [showConfirmationModal, setShowConfirmationModal] = useState<boolean>(false)

  const { register, handleSubmit, reset } = useForm<ICallMeBackForm>()

  const onSubmit = async (formData: ICallMeBackForm) => {
    await store.postCallMeBackData(formData, postCallMeBackUrl)
    if (store.isSuccess) {
      reset()
      setShowConfirmationModal(true)
    }
  }

  const handleCloseModal = () => {
    onClose()
    setShowConfirmationModal(false)
    setNoteOpen(false)
    store.setError(false)
  }

  /* Form modal body.*/
  const formModalBody = () =>
    !store.isLoading &&
    !store.error && (
      <>
        <Modal.Body className="d-flex flex-column align-items-center justify-content-between">
          <h2 className="cmb__heading">{store.getResponseDetails?.title || ''}</h2>
          <img
            aria-hidden
            className="cmb__image"
            loading="lazy"
            decoding="async"
            alt=""
            src={store.getResponseDetails?.image.url || ''}
          />
          <Row className="justify-content-center w-100">
            <Col xs={12} lg={6} className="d-flex flex-column">
              <form onSubmit={handleSubmit(onSubmit)}>
                {/* Phone input. */}
                <Form.Group>
                  <Form.Label htmlFor={phoneId}>
                    {store.getResponseDetails?.labels?.PHONE}
                  </Form.Label>
                  <Form.Control
                    defaultValue={store.getResponseDetails?.phone}
                    as="input"
                    type="text"
                    inputMode="numeric"
                    id={phoneId}
                    pattern="^((\+|00)420 {0,1})?[2-9](\d{2}) {0,1}(\d{3}) {0,1}(\d{3})$"
                    title={store.getResponseDetails?.labels.VALIDATION_ERROR}
                    required
                    {...register('phone', {})}
                  />
                </Form.Group>
                {/* Phone input. */}
                {/* Time input. */}
                <Form.Group>
                  <Form.Label htmlFor={timeId}>
                    {store.getResponseDetails?.labels?.TIME_LABEL || ''}
                  </Form.Label>
                  <Form.Control {...register('time', {})} as="select" id={timeId}>
                    <option>{store.getResponseDetails?.labels?.TIME_PLACEHOLDER || ''}</option>
                    {store.getResponseDetails?.times.map(({ label, value }, idx) => (
                      <option key={idx} value={value}>
                        {label}
                      </option>
                    ))}
                  </Form.Control>
                </Form.Group>
                {/* Time input. */}
                {/* Add note. */}
                {!noteOpen ? (
                  <Button
                    aria-expanded={false}
                    onClick={() => setNoteOpen(true)}
                    variant="link"
                    className="cmb__additional"
                  >
                    {store.getResponseDetails?.labels?.ADD_NOTE || ''}
                  </Button>
                ) : (
                  <Form.Group>
                    <Form.Label htmlFor="note">
                      {store.getResponseDetails?.labels?.NOTE || ''}
                    </Form.Label>
                    <div className="d-flex w-100 position-relative">
                      <Form.Control as="input" type="text" id="note" {...register('note', {})} />
                      <Button
                        aria-label={store.getResponseDetails?.labels?.CLOSE_BUTTON || ''}
                        aria-expanded={true}
                        className="close cmb__additionalCloseBtn"
                        variant="none"
                        onClick={() => setNoteOpen(false)}
                      >
                        <span aria-hidden="true">×</span>
                      </Button>
                    </div>
                  </Form.Group>
                )}
                {/* Add note. */}
                {/* Submit button. */}
                <Button variant="primary" type="submit" className="cmb__submitBtn">
                  {store.getResponseDetails?.labels?.SUBMIT_BUTTON || ''}
                </Button>
                {/* Submit button. */}
              </form>
            </Col>
          </Row>
          {/* Disclaimer. */}
          <div
            dangerouslySetInnerHTML={{
              __html: store.getResponseDetails?.labels?.BOTTOM_TEXT || '',
            }}
          />
          {/* Disclaimer. */}
        </Modal.Body>
      </>
    )
  /* Form modal body. */

  /* Confirmation modal body. */
  const confirmationModalBody = () => (
    <Modal.Body>
      <Row className="justify-content-center w-100">
        <Col xs={12} lg={6} className="d-flex flex-column">
          <div role="alert">
            <h2> {store.postResponseDetails?.labels.TITLE}</h2>
            <p className="mb-4">{store.postResponseDetails?.labels.TEXT}</p>
          </div>
          <span className="cmb__iconWrapper">
            <Icon name="check-circle" size={48} color={colors.orange}></Icon>
          </span>
          <Button onClick={handleCloseModal} variant="primary">
            {store.postResponseDetails?.labels.CLOSE}
          </Button>
        </Col>
      </Row>
    </Modal.Body>
  )
  /* Confirmation modal body. */

  return (
    <Modal size="lg" show={show} onHide={handleCloseModal}>
      <div className={classNames({ loading: store.isLoading }, 'cmb')}>
        <Modal.Header className="cmb__close" closeButton />
        {store.isSuccess && !store.isLoading && showConfirmationModal
          ? confirmationModalBody()
          : formModalBody()}
        {store.error && (
          <div className="cmb__errorMessage" role="alert">
            {store.errorMessage}
          </div>
        )}
      </div>
    </Modal>
  )
})
