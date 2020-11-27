import React, { useEffect, useState, useCallback, useRef, Fragment } from 'react'
import { OfferType, View } from '@types'
import { observer } from 'mobx-react-lite'
import { OfferStore } from '@stores'
import { Alert, Button, Col, Form, Row, Table } from 'react-bootstrap'
import classNames from 'classnames'
import {
  Box,
  BoxHeading,
  ConfirmationModal,
  FileUpload,
  FormCheckWrapper,
  Gift,
  Icon,
  SignatureModal,
  UploadZone,
} from '@components'
import { colors } from '@theme'
import { useLabels } from '@hooks'
import { OfferStoreContext } from '@context'

type SignatureModalType = {
  id: string
  show: boolean
}

export const Offer: React.FC<View> = observer(
  ({
    offerUrl,
    labels,
    formAction,
    getFileUrl,
    getFileForSignUrl,
    signFileUrl,
    timeout,
    uploadFileUrl,
    removeFileUrl,
    errorPageUrl,
    allowedContentTypes,
    maxFileSize,
  }) => {
    const [store] = useState(() => new OfferStore(OfferType.NEW, offerUrl))
    const [signatureModalProps, setSignatureModalProps] = useState<SignatureModalType>({
      id: '',
      show: false,
    })
    const [confirmationModal, setConfirmationModal] = useState(false)
    const t = useLabels(labels)
    const formRef = useRef<HTMLFormElement>(null)

    useEffect(() => {
      store.errorPageUrl = errorPageUrl
      store.fetchOffer(timeout)

      // set correct upload document URL if provided
      if (uploadFileUrl) {
        store.uploadDocumentUrl = uploadFileUrl
      }

      if (removeFileUrl) {
        store.removeDocumentUrl = removeFileUrl
      }
      // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    const openSignatureModal = useCallback((id: string) => {
      setSignatureModalProps({
        id,
        show: true,
      })
    }, [])

    const handleAcceptOffer = useCallback(() => {
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

      formRef.current?.submit()
    }, [])

    return (
      <OfferStoreContext.Provider value={store}>
        {/* error state */}
        {store.error && (
          <Alert variant="danger">
            <h3>{t('appUnavailableTitle')}</h3>
            <div>{t('appUnavailableText')}</div>
          </Alert>
        )}
        {/* /error state */}

        <form
          action={formAction}
          method="post"
          ref={formRef}
          className={classNames({
            'bg--gray-10': store.isLoading,
            loading: store.isLoading,
            'd-none': store.error, // hide the whole form if there is an error
          })}
        >
          {/* summary / perex box */}
          {store.perex && (
            <Box backgroundColor="gray-80">
              <BoxHeading>{store.perex.title}</BoxHeading>
              {store.perex.params.length > 0 && (
                <Table size="sm" borderless>
                  <tbody>
                    {store.perex.params.map(({ title, value }, idx) => (
                      <tr key={idx}>
                        <th scope="row" className="w-50 text-right font-weight-normal">
                          {title}:
                        </th>
                        <td className="w-50 text-left font-weight-bold">{value}</td>
                      </tr>
                    ))}
                  </tbody>
                </Table>
              )}
            </Box>
          )}
          {/* /summary / perex box */}

          {/* benefits box */}
          {store.benefits && (
            <Box backgroundColor="purple-light">
              <BoxHeading>{store.benefits.title}</BoxHeading>
              {store.benefits.params.length > 0 && (
                <Row as="ul" className="justify-content-center list-unstyled mb-0">
                  {store.benefits.params.map(({ value }, idx) => (
                    <Col as="li" key={idx} xs={12} sm={6} lg={4} className="my-3 text-center">
                      <Icon name="check-circle" size={40} color={colors.white} />
                      <span className="d-block mt-2 font-weight-bold">{value}</span>
                    </Col>
                  ))}
                </Row>
              )}
            </Box>
          )}
          {/* /benefits box */}

          {/* gifts box */}
          {store.gifts && (
            <Box backgroundColor="green">
              <BoxHeading>
                {store.singleGiftGroup ? store.gifts.groups[0].title : store.gifts.title}
              </BoxHeading>
              {store.singleGiftGroup ? (
                <Row as="ul" className="justify-content-center list-unstyled mb-0">
                  {store.gifts.groups[0].params.map(({ title, icon }, idx) => (
                    <Col as="li" key={idx} xs={12} sm={6} lg={4} className="mb-3">
                      <Gift type={icon} title={title} />
                    </Col>
                  ))}
                </Row>
              ) : (
                <Row>
                  {store.gifts.groups.map(({ title, params }, idx) => (
                    <Col
                      key={idx}
                      xs={12}
                      md={6}
                      // show a different number of columns based on groups length
                      xl={store.gifts && store.gifts.groups.length > 2 ? 4 : undefined}
                      className="d-flex flex-column mb-5"
                    >
                      <h3 className="h4 mb-4 gift__group-title">{title}</h3>
                      {params.length > 0 && (
                        <ul className="list-unstyled mb-0">
                          {params.map(({ title, icon }, idx) => (
                            <li key={idx} className="mb-3">
                              <Gift type={icon} title={title} />
                            </li>
                          ))}
                        </ul>
                      )}
                    </Col>
                  ))}
                </Row>
              )}
              <p className="text-center">{store.gifts.note}</p>
            </Box>
          )}
          {/* /gifts box */}

          {/* box with documents to be accepted or signed */}
          {store.documents.acceptance && (
            <>
              <h2 className="mt-5">{store.documents.acceptance.title}</h2>
              <div
                className="editorial-content"
                dangerouslySetInnerHTML={{ __html: store.documents.acceptance.text }}
              />
              <Box>
                {store.documentsToBeAccepted.length > 0 && (
                  <>
                    <BoxHeading>{store.documents.acceptance?.accept?.title}</BoxHeading>
                    <div
                      className="my-4 text-center editorial-content"
                      dangerouslySetInnerHTML={{
                        __html: store.documents.acceptance?.accept?.subTitle ?? '',
                      }}
                    />
                    <div className="mb-2">
                      <Button
                        variant="link"
                        onClick={() => store.acceptAllDocuments(store.documentsToBeAccepted)}
                      >
                        {t('acceptAll')}
                      </Button>
                    </div>
                    {store.documentsToBeAccepted.map(({ key, prefix, accepted, label }) => (
                      <FormCheckWrapper
                        key={key}
                        type="checkbox"
                        name="acceptedDocuments"
                        id={`document-${key}`}
                        value={key}
                        checked={accepted !== undefined ? accepted : false}
                        onChange={() => store.acceptDocument(key)}
                      >
                        <Form.Check.Label>
                          <span>
                            {prefix} <a href={`${getFileUrl}${key}`}>{label}</a>
                          </span>
                          <Icon
                            name="pdf"
                            size={36}
                            color={colors.orange}
                            className="ml-md-auto d-none d-sm-block"
                          />
                        </Form.Check.Label>
                      </FormCheckWrapper>
                    ))}
                  </>
                )}

                {store.documentsToBeSigned.length > 0 && (
                  <>
                    <BoxHeading>{store.documents.acceptance.sign?.title}</BoxHeading>
                    <p className="my-4 text-center">{store.documents.acceptance?.sign?.subTitle}</p>
                    {store.documentsToBeSigned.map(({ key, prefix, label, signed }) => (
                      <div key={key} className="form-item-wrapper mb-3">
                        <div className="like-custom-control-label">
                          {signed && (
                            <Icon
                              name="check-circle"
                              size={36}
                              color={colors.green}
                              className="form-item-wrapper__icon mr-2"
                            />
                          )}
                          <span>
                            {prefix} <a href={`${getFileUrl}${key}`}>{label}</a>
                          </span>
                          {signed ? (
                            <Button
                              variant="primary"
                              className="btn-icon ml-auto form-item-wrapper__btn"
                              aria-label={t('signatureEditBtn')}
                              onClick={() => openSignatureModal(key)}
                            >
                              <Icon name="edit" size={18} color={colors.white} />
                            </Button>
                          ) : (
                            <Button
                              variant="secondary"
                              className="ml-auto form-item-wrapper__btn"
                              onClick={() => openSignatureModal(key)}
                            >
                              {t('signatureBtn')}
                            </Button>
                          )}
                        </div>
                      </div>
                    ))}
                    <div
                      className="editorial-content text-muted small"
                      dangerouslySetInnerHTML={{
                        __html: store.documents.acceptance.sign?.note ?? '',
                      }}
                    />
                  </>
                )}
              </Box>
            </>
          )}
          {/* /box with documents to be accepted or signed */}

          {/* uploads box */}
          {store.documents.uploads && (
            <>
              <h2 className="mt-5">{store.documents.uploads.title}</h2>
              <Box>
                {store.documents.uploads.types.map(({ id: categoryId, info, title, mandatory }) => (
                  <div key={categoryId} className="mb-5">
                    <UploadZone
                      mandatory={mandatory}
                      label={title}
                      labelTooltip={info}
                      labels={labels}
                      allowedContentTypes={allowedContentTypes}
                      maxFileSize={maxFileSize}
                      onFilesAccepted={files => store.addUserFiles(files, categoryId)}
                    />
                    {/* custom uploaded documents */}
                    {store.userDocuments[categoryId]?.length > 0 && (
                      <ul aria-label={t('selectedFiles')} className="list-unstyled">
                        {store.userDocuments[categoryId].map(document => (
                          <li key={document.key}>
                            <FileUpload
                              file={document.file}
                              labels={labels}
                              onRemove={() => {
                                store.cancelUploadDocument(document)
                                store.removeUserDocument(document.key, categoryId)
                              }}
                              uploadHandler={() => store.uploadDocument(document, categoryId)}
                              // do not allow to reupload of already touched file (both with success or error)
                              shouldUploadImmediately={!document.touched}
                              error={document.error}
                              uploading={document.uploading}
                            />
                          </li>
                        ))}
                      </ul>
                    )}
                    {/* /custom uploaded documents */}
                  </div>
                ))}
                <div
                  className="small text-muted editorial-content"
                  dangerouslySetInnerHTML={{ __html: store.documents.uploads.note }}
                />
                {/* <p className="small text-muted">Dokumenty označené * jsou povinné.</p> */}
              </Box>
            </>
          )}
          {/* /uploads box */}

          {/* products box */}
          {store.documents.other?.products && (
            <>
              <h2 className="mt-5">{store.documents.other.products.title}</h2>
              <Box>
                <BoxHeading>{store.documents.other.products.subTitle}</BoxHeading>
                {store.documents.other.products.params.length > 0 && (
                  <Table size="sm" borderless>
                    <tbody>
                      {store.documents.other.products.params.map(({ title, value }, idx) => (
                        <tr key={idx}>
                          <th scope="row" className="w-50 text-right font-weight-normal">
                            {title}:
                          </th>
                          <td className="w-50 text-left font-weight-bold">{value}</td>
                        </tr>
                      ))}
                    </tbody>
                  </Table>
                )}

                {store.documents.other.products.arguments.length > 0 && (
                  <Box backgroundColor="blue-green-light">
                    <Row as="ul" className="justify-content-center list-unstyled mb-0">
                      {store.documents.other.products.arguments.map(({ value }, idx) => (
                        <Col as="li" key={idx} xs={12} sm={6} lg={4} className="my-3 text-center">
                          <Icon name="check-circle" size={40} color={colors.white} />
                          <span className="d-block mt-2 font-weight-bold">{value}</span>
                        </Col>
                      ))}
                    </Row>
                  </Box>
                )}

                <BoxHeading>{store.documents.other.products.subTitle2}</BoxHeading>
                <p className="text-center my-4">Dokument(y) si pročtěte a potvrďte zatržením</p>
                <div className="mb-2">
                  <Button
                    variant="link"
                    onClick={() => store.acceptAllDocuments(store.documentsProducts)}
                  >
                    {t('acceptAll')}
                  </Button>
                </div>
                {store.documentsProducts.map(({ key, prefix, label, accepted }) => (
                  <FormCheckWrapper
                    key={key}
                    type="checkbox"
                    name="commoditiesDocuments"
                    id={`document-${key}`}
                    value={key}
                    checked={accepted !== undefined ? accepted : false}
                    onChange={() => store.acceptDocument(key)}
                  >
                    <Form.Check.Label>
                      <span>
                        {prefix} <a href={`${getFileUrl}${key}`}>{label}</a>
                      </span>
                      <Icon
                        name="pdf"
                        size={36}
                        color={colors.orange}
                        className="ml-md-auto d-none d-sm-block"
                      />
                    </Form.Check.Label>
                  </FormCheckWrapper>
                ))}
                <div className="text-center mt-4">
                  <Icon
                    name="info-circle"
                    size={40}
                    color={colors.gray100}
                    className="d-block mx-auto mb-3"
                  />
                  <div
                    className="editorial-content"
                    dangerouslySetInnerHTML={{ __html: store.documents.other.products.note }}
                  />
                </div>
              </Box>
            </>
          )}
          {/* /products box */}

          {/* services box */}
          {store.documents.other?.services && (
            <>
              <h2 className="mt-5">{store.documents.other.services.title}</h2>
              <Box>
                <p className="text-center mt-2 mb-4">
                  Dokument(y) si pročtěte a potvrďte zatržením
                </p>
                <div className="mb-2">
                  <Button
                    variant="link"
                    onClick={() => store.acceptAllDocuments(store.documentsServices)}
                  >
                    {t('acceptAll')}
                  </Button>
                </div>
                {store.documentsServices.map(({ key, prefix, label, accepted }) => (
                  <FormCheckWrapper
                    key={key}
                    type="checkbox"
                    name="commoditiesDocuments"
                    id={`document-${key}`}
                    value={key}
                    checked={accepted !== undefined ? accepted : false}
                    onChange={() => store.acceptDocument(key)}
                  >
                    <Form.Check.Label>
                      <span>
                        {prefix} <a href={`${getFileUrl}${key}`}>{label}</a>
                      </span>
                      <Icon
                        name="pdf"
                        size={36}
                        color={colors.orange}
                        className="ml-md-auto d-none d-sm-block"
                      />
                    </Form.Check.Label>
                  </FormCheckWrapper>
                ))}
              </Box>
            </>
          )}
          {/* /services box */}

          {/**
           * submit zone
           * Hide the zone if there is an error or we didn't receive any documents.
           */}
          <div
            className={classNames({
              'd-none': (!store.isLoading && store.error) || !store.offerFetched,
            })}
          >
            <h2 className="mt-5">{t('acceptOfferTitle')}</h2>
            <Box className="text-center">
              <p>{t('acceptOfferHelptext')}</p>
              <Button
                variant="secondary"
                onClick={() => setConfirmationModal(true)}
                disabled={!store.isOfferReadyToAccept}
              >
                {t('submitBtn')}
              </Button>
            </Box>
          </div>
          {/* submit zone */}
        </form>

        <SignatureModal
          {...signatureModalProps}
          onClose={() => setSignatureModalProps({ show: false, id: '' })}
          labels={labels}
          getFileForSignUrl={getFileForSignUrl ?? ''}
          signFileUrl={signFileUrl ?? ''}
        />

        <ConfirmationModal
          show={confirmationModal}
          onClose={() => setConfirmationModal(false)}
          onConfirm={handleAcceptOffer}
          labels={labels}
        />
      </OfferStoreContext.Provider>
    )
  },
)
