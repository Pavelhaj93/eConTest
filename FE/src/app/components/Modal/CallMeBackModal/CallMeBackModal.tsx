import React, { ChangeEvent, useContext, useEffect } from 'react'
import { observer } from 'mobx-react-lite'
import { Button, Col, Form, Modal, Row } from 'react-bootstrap'
import classNames from 'classnames'
import { colors } from '@theme'
import { ICallMeBackForm } from '../../../types/CallMeBack'
import { CallMeBackStoreContext } from '@context'
import { CallMeBackStore } from '@stores'
import { Icon } from '../../Icon'
import { generateId, isGetResponseSucceed } from '@utils'
import { UploadFileInput } from './UploadFileInput'

type CallMeBackModalProps = {
  show: boolean
  onClose: () => void
  postCallMeBackUrl: string
}

/* ID for phone input. */
const phoneId = `phone_${generateId()}`
/* ID for select input. */
const timeId = `when_${generateId()}`

export const CallMeBackModal: React.FC<CallMeBackModalProps> = observer(
  ({ show, onClose, postCallMeBackUrl }) => {
    const store = useContext(CallMeBackStoreContext)

    if (!(store instanceof CallMeBackStore)) {
      return null
    }

    /* Form submit function. */
    const handleSubmit = async () => {
      const currentBrowserUrl = typeof window !== 'undefined' ? window.location.href : ''
      const formData: ICallMeBackForm = {
        phone: store.phoneValue,
        time: store.timeValue,
        note: store.noteValue,
        file: [store.fileA, store.fileB],
        currentBrowserUrl,
      }

      await store.postCallMeBackData(formData, postCallMeBackUrl)

      if (store.isSuccess) {
        store.setShowConfirmationModal(true)
      }
    }

    /* Actions on close modal. */
    const handleCloseModal = () => {
      onClose()
      store.handleCloseModalStoreValue()
    }

    useEffect(() => {
      /* Set default phone value. */
      if (isGetResponseSucceed(store.getResponseDetails)) {
        store.setPhoneValue(store.getResponseDetails.phone)
      }
    }, [store, store.getResponseDetails])

    /* Form modal body.*/
    const formModalBody = () =>
      isGetResponseSucceed(store.getResponseDetails) &&
      !store.isLoading &&
      !store.error && (
        <>
          <Modal.Body className="d-flex flex-column align-items-center justify-content-between">
            <h2 className="cmb__heading">{store.getResponseDetails.title}</h2>
            {store.getResponseDetails.image && (
              <img
                aria-hidden
                className="cmb__image"
                loading="lazy"
                decoding="async"
                alt=""
                src={store.getResponseDetails?.image.url || ''}
              />
            )}
            <Row className="justify-content-center w-100">
              <Col xs={12} lg={8} className="d-flex flex-column">
                <form className="d-flex flex-column" onSubmit={handleSubmit}>
                  {/* Phone input. */}
                  <Form.Group>
                    <Form.Label htmlFor={phoneId}>
                      {store.getResponseDetails.labels.PHONE}
                    </Form.Label>
                    <Form.Control
                      as="input"
                      type="text"
                      inputMode="numeric"
                      id={phoneId}
                      pattern="^((\+|00)420 {0,1})?[2-9](\d{2}) {0,1}(\d{3}) {0,1}(\d{3})$"
                      title={store.getResponseDetails?.labels.VALIDATION_ERROR || ''}
                      required
                      value={store.phoneValue}
                      onChange={event => store.setPhoneValue(event.target.value)}
                    />
                  </Form.Group>
                  {/* Phone input. */}
                  {/* Time input. */}
                  <Form.Group>
                    <Form.Label htmlFor={timeId}>
                      {store.getResponseDetails?.labels?.TIME_LABEL || ''}
                    </Form.Label>
                    <Form.Control
                      className="text-center"
                      value={store.timeValue}
                      onChange={event => store.setTimeValue(event.target.value)}
                      as="select"
                      id={timeId}
                    >
                      <option>{store.getResponseDetails?.labels?.TIME_PLACEHOLDER || ''}</option>
                      {store.getResponseDetails?.times?.map(({ label, value }, idx) => (
                        <option key={idx} value={value}>
                          {label}
                        </option>
                      ))}
                    </Form.Control>
                  </Form.Group>
                  {/* Time input. */}
                  {/* Add note. */}
                  {!store.isNoteOpen ? (
                    <Button
                      aria-expanded={false}
                      onClick={() => store?.setIsNoteOpen(true)}
                      variant="link"
                      className="cmb__addNote"
                    >
                      {store.getResponseDetails?.labels.ADD_NOTE || ''}
                    </Button>
                  ) : (
                    <Form.Group>
                      <Form.Label htmlFor="note">
                        {store.getResponseDetails?.labels.NOTE || ''}
                      </Form.Label>
                      <div className="d-flex w-100 position-relative">
                        <Form.Control
                          value={store.noteValue}
                          className="cmb__note"
                          as="textarea"
                          id="note"
                          onChange={event => store.setNoteValue(event.target.value)}
                        />
                      </div>
                    </Form.Group>
                  )}
                  {/* Add note. */}
                  {/* Add files. */}
                  <p className="cmb__fileLabel">
                    {store.getResponseDetails?.labels.ADD_FILE || ''}
                  </p>
                  <UploadFileInput
                    file={store.fileA}
                    onChange={(event: ChangeEvent) => store.handleSetFile(event, 'A')}
                    onRemoveFile={() => store.setFileA(undefined)}
                    acceptedFormat={store.getResponseDetails?.allowedFiles?.join(',') || ''}
                    labels={{
                      insertFile: store.getResponseDetails?.labels?.INSERT_FILE || '',
                      removeFile: store.getResponseDetails?.labels?.REMOVE_FILE || '',
                    }}
                    className="mb-2"
                    confirmationClassName="mt-2 mb-2"
                  />
                  {!store.isFileOpen ? (
                    <Button
                      aria-expanded={false}
                      onClick={() => store?.setIsFileOpen(true)}
                      variant="link"
                      className="cmb__additional"
                    >
                      <Icon name="plus-circle" color={colors.white} className="mr-2" size={40} />
                      <span>{store.getResponseDetails?.labels.ADD_FILE || ''}</span>
                    </Button>
                  ) : (
                    <UploadFileInput
                      file={store.fileB}
                      onChange={(event: ChangeEvent) => store.handleSetFile(event, 'B')}
                      onRemoveFile={() => store.setFileB(undefined)}
                      acceptedFormat={store.getResponseDetails?.allowedFiles?.join(',') || ''}
                      labels={{
                        insertFile: store.getResponseDetails?.labels?.INSERT_FILE || '',
                        removeFile: store.getResponseDetails?.labels?.REMOVE_FILE || '',
                      }}
                      confirmationClassName="mb-4"
                    />
                  )}
                  {store.isFileSizeError && (
                    <div className="cmb__validationError" role="alert">
                      {store.getResponseDetails?.labels.FILE_SIZE_ERROR}
                    </div>
                  )}
                  {/* Add files. */}

                  {/* Files note. */}
                  <div
                    className="text-center font-weight-bold mb-3 text-sm"
                    dangerouslySetInnerHTML={{
                      __html: store.getResponseDetails?.labels?.FILES_NOTE || '',
                    }}
                  />
                  {/* Files note. */}

                  {/* Submit button. */}
                  <Button variant="primary" type="submit" className="cmb__submitBtn">
                    {store?.getResponseDetails?.labels?.SUBMIT_BUTTON || ''}
                  </Button>
                  {/* Submit button. */}
                </form>
              </Col>
            </Row>
            {/* Disclaimer. */}
            <div
              className="text-center font-weight-bold"
              dangerouslySetInnerHTML={{
                __html: store?.getResponseDetails?.labels.BOTTOM_TEXT || '',
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
          <Col xs={12} lg={8} className="d-flex flex-column">
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
          {store.isSuccess && !store.isLoading && store.showConfirmationModal
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
  },
)
