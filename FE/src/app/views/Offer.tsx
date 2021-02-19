import React, { useEffect, useState, useCallback, useRef, FormEvent } from 'react'
import { OfferType, View } from '@types'
import { observer } from 'mobx-react-lite'
import { OfferStore } from '@stores'
import { Alert, Button, Col, Form, Row, Table } from 'react-bootstrap'
import classNames from 'classnames'
import Media from 'react-media'
import {
  Box,
  BoxHeading,
  ConfirmationModal,
  DocumentLink,
  FileUpload,
  FormCheckWrapper,
  Gift,
  Icon,
  SignatureModal,
  SignButton,
  UploadZone,
} from '@components'
import { breakpoints, colors } from '@theme'
import { useLabels, useUnload } from '@hooks'
import { OfferStoreContext } from '@context'
import { isIE11 } from '@utils'

type SignatureModalType = {
  id: string
  show: boolean
}

export const Offer: React.FC<View> = observer(
  ({
    offerUrl,
    labels,
    getFileUrl,
    thumbnailUrl,
    signFileUrl,
    timeout,
    uploadFileUrl,
    removeFileUrl,
    errorPageUrl,
    allowedContentTypes,
    maxFileSize,
    maxGroupFileSize,
    acceptOfferUrl,
    thankYouPageUrl,
    sessionExpiredPageUrl,
  }) => {
    const [store] = useState(() => new OfferStore(OfferType.NEW, offerUrl))
    const [signatureModalProps, setSignatureModalProps] = useState<SignatureModalType>({
      id: '',
      show: false,
    })
    const [confirmationModal, setConfirmationModal] = useState(false)
    const t = useLabels(labels)
    const formRef = useRef<HTMLFormElement>(null)

    // keep session alive
    // useKeepAlive(30 * 1000, keepAliveUrl)

    useEffect(() => {
      store.errorPageUrl = errorPageUrl
      store.sessionExpiredPageUrl = sessionExpiredPageUrl
      store.fetchOffer(timeout)

      // set correct upload document URL if provided
      if (uploadFileUrl) {
        store.uploadDocumentUrl = uploadFileUrl
      }

      if (removeFileUrl) {
        store.removeDocumentUrl = removeFileUrl
      }

      if (maxGroupFileSize) {
        store.maxUploadGroupSize = maxGroupFileSize
      }

      if (acceptOfferUrl) {
        store.acceptOfferUrl = acceptOfferUrl
      }
      // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    // show warning to user when trying to refresh or leave the page once he did some changes
    useUnload(ev => {
      if (store.isOfferDirty && !store.isAccepting && !store.forceReload) {
        ev.preventDefault()
        ev.returnValue = ''
      }
    })

    const openSignatureModal = useCallback((id: string) => {
      setSignatureModalProps({
        id,
        show: true,
      })
    }, [])

    // Since IE11 doesn't support a `download` attribute on link, here I manually change the `forceReload`
    // property so browser can refresh and download the file.
    const handleDownload = useCallback(() => {
      if (isIE11()) {
        store.forceReload = true
      }
    }, [store])

    return (
      <OfferStoreContext.Provider value={store}>
        {/* error state */}
        {store.error && (
          <Alert variant="danger">
            <h3>{t('appUnavailableTitle')}</h3>
            <div
              data-testid="errorMessage"
              className="editorial-content"
              dangerouslySetInnerHTML={{ __html: store.errorMessage || t('appUnavailableText') }}
            />
          </Alert>
        )}
        {/* /error state */}

        <form
          action={acceptOfferUrl}
          method="post"
          ref={formRef}
          className={classNames({
            'bg--gray-10': store.isLoading,
            loading: store.isLoading,
            'd-none': store.error, // hide the whole form if there is an error
          })}
          onSubmit={(ev: FormEvent) => ev.preventDefault()}
        >
          {/* summary / perex box */}
          {store.perex && (
            <Box backgroundColor="gray-80">
              <BoxHeading>{store.perex.title}</BoxHeading>
              {store.perex.params.length > 0 && (
                <Table
                  size="sm"
                  borderless
                  data-testid="summaryTable"
                  className="table-two-columns"
                >
                  <tbody>
                    {store.perex.params.map(({ title, value }, idx) => (
                      <tr key={idx}>
                        <th scope="row">{title}:</th>
                        <td>{value}</td>
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
              <BoxHeading data-testid="giftBoxHeading">
                {store.singleGiftGroup ? store.gifts.groups[0].title : store.gifts.title}
              </BoxHeading>
              {store.singleGiftGroup ? (
                <Row as="ul" className="justify-content-center list-unstyled mb-0">
                  {store.gifts.groups[0].params.map(({ title, icon, count }, idx) => (
                    <Col
                      as="li"
                      key={idx}
                      xs={12}
                      // render a different width if there is only one gift
                      sm={store.gifts?.groups[0].params.length !== 1 ? 6 : undefined}
                      md={store.gifts?.groups[0].params.length === 1 ? 8 : undefined}
                      lg={store.gifts?.groups[0].params.length !== 1 ? 4 : undefined}
                      className="mb-3"
                    >
                      <Gift
                        type={icon}
                        title={`${count}x ${title}`}
                        className="justify-content-sm-center"
                      />
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
                      <h4 className="mb-4 gift__group-title">{title}</h4>
                      {params.length > 0 && (
                        <ul className="list-unstyled mb-0">
                          {params.map(({ title, icon, count }, idx) => (
                            <li key={idx} className="mb-3">
                              <Gift type={icon} title={`${count}x ${title}`} />
                            </li>
                          ))}
                        </ul>
                      )}
                    </Col>
                  ))}
                </Row>
              )}
              <div
                className="text-center editorial-content"
                dangerouslySetInnerHTML={{ __html: store.gifts.note }}
              />
            </Box>
          )}
          {/* /gifts box */}

          {/* box with documents to be accepted or signed */}
          {(store.documents.acceptance?.accept || store.documents.acceptance?.sign) && (
            <>
              <h2 className="mt-5">{store.documents.acceptance.title}</h2>
              <div
                className="editorial-content"
                dangerouslySetInnerHTML={{ __html: store.documents.acceptance.text }}
              />
              <Box data-testid="boxDocumentsToBeAccepted">
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
                        aria-pressed={store.allDocumentsAreAccepted}
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
                          <span className="mr-1">{prefix}</span>
                          <DocumentLink
                            url={`${getFileUrl}/${key}`}
                            label={label}
                            onClick={handleDownload}
                          />
                        </Form.Check.Label>
                      </FormCheckWrapper>
                    ))}
                  </>
                )}

                {store.documentsToBeSigned.length > 0 && (
                  <>
                    <BoxHeading>{store.documents.acceptance.sign?.title}</BoxHeading>
                    <div
                      className="editorial-content text-center my-4"
                      dangerouslySetInnerHTML={{
                        __html: store.documents.acceptance?.sign?.subTitle ?? '',
                      }}
                    />
                    {store.documentsToBeSigned.map(({ key, prefix, label, accepted }) => (
                      <div key={key} className="form-item-wrapper mb-3">
                        <div className="like-custom-control-label">
                          {accepted && (
                            <Icon
                              name="check-circle"
                              size={36}
                              color={colors.green}
                              className="form-item-wrapper__icon mr-2"
                            />
                          )}
                          <span>
                            {prefix}{' '}
                            <DocumentLink
                              url={`${getFileUrl}/${key}?t=${new Date().getTime()}`}
                              label={label}
                              onClick={handleDownload}
                              noIcon
                            />
                          </span>
                          <SignButton
                            className="d-none d-sm-block"
                            signed={accepted}
                            onClick={() => openSignatureModal(key)}
                            labelSign={t('signatureBtn')}
                            labelEdit={t('signatureEditBtn')}
                            descriptionId={'signBtnDescription'}
                            showLabelEdit={false}
                          />
                        </div>
                        <Media query={{ maxWidth: breakpoints.smMax }}>
                          {matches =>
                            matches && (
                              <SignButton
                                className="btn-block-mobile mt-3"
                                signed={accepted}
                                onClick={() => openSignatureModal(key)}
                                labelSign={t('signatureBtn')}
                                labelEdit={t('signatureEditBtn')}
                                descriptionId={'signBtnDescription'}
                                showLabelEdit={true}
                              />
                            )
                          }
                        </Media>
                      </div>
                    ))}
                    <div
                      id="signBtnDescription"
                      className="editorial-content text-muted small"
                      dangerouslySetInnerHTML={{
                        __html: t('signatureNote'),
                      }}
                      aria-hidden="true"
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
                      disabled={store.uploadGroupSizeExceeded}
                    />
                    {/* custom uploaded documents */}
                    {store.userDocuments[categoryId]?.length > 0 && (
                      <ul aria-label={t('selectedFiles')} className="list-unstyled">
                        {store.userDocuments[categoryId].map(document => (
                          <li key={document.key} className={classNames({ shake: document.error })}>
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

          {/* services box */}
          {store.documents.other?.services && (
            <>
              <h2 className="mt-5">{store.documents.other.services.title}</h2>
              <Box>
                {store.documents.other.services.subTitle &&
                  (store.documents.other.services.params ||
                    store.documents.other.services.arguments) && (
                    <BoxHeading>{store.documents.other.services.subTitle}</BoxHeading>
                  )}

                {store.documents.other.services.params &&
                  store.documents.other.services.params.length > 0 && (
                    <Table size="sm" className="table-two-columns" borderless>
                      <tbody>
                        {store.documents.other.services.params.map(({ title, value }, idx) => (
                          <tr key={idx}>
                            <th scope="row">{title}:</th>
                            <td>{value}</td>
                          </tr>
                        ))}
                      </tbody>
                    </Table>
                  )}

                {store.documents.other.services.arguments &&
                  store.documents.other.services.arguments.length > 0 && (
                    <Box backgroundColor="blue-green-light">
                      <Row
                        as="ul"
                        className="justify-content-center list-unstyled mb-0"
                        aria-label={t('productBenefits')}
                      >
                        {store.documents.other.services.arguments.map(({ value }, idx) => (
                          <Col as="li" key={idx} xs={12} sm={6} lg={4} className="my-3 text-center">
                            <Icon name="check-circle" size={40} color={colors.white} />
                            <span className="d-block mt-2 font-weight-bold">{value}</span>
                          </Col>
                        ))}
                      </Row>
                    </Box>
                  )}

                {store.documents.other.services.subTitle2 && (
                  <BoxHeading>{store.documents.other.services.subTitle2}</BoxHeading>
                )}
                <div
                  className="text-center editorial-content mt-2 mb-4"
                  dangerouslySetInnerHTML={{ __html: store.documents.other.services.text }}
                />
                <div className="mb-2">
                  <Button
                    variant="link"
                    onClick={() => store.acceptAllDocuments(store.documentsServices)}
                    aria-pressed={store.documentsServices.every(d => d.accepted)}
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
                      <span className="mr-1">{prefix}</span>
                      <DocumentLink
                        url={`${getFileUrl}/${key}`}
                        label={label}
                        onClick={handleDownload}
                      />
                    </Form.Check.Label>
                  </FormCheckWrapper>
                ))}
                {/* info text */}
                {store.documents.other.services.note && (
                  <div className="text-center mt-4">
                    <Icon
                      name="info-circle"
                      size={40}
                      color={colors.gray100}
                      className="d-block mx-auto mb-3"
                    />
                    <div
                      className="editorial-content"
                      dangerouslySetInnerHTML={{ __html: store.documents.other.services.note }}
                    />
                  </div>
                )}
              </Box>
            </>
          )}
          {/* /services box */}

          {/* products box */}
          {store.documents.other?.products && (
            <>
              <h2 className="mt-5">{store.documents.other.products.title}</h2>
              <Box>
                <BoxHeading>{store.documents.other.products.subTitle}</BoxHeading>
                {store.documents.other.products.params.length > 0 && (
                  <Table size="sm" className="table-two-columns" borderless>
                    <tbody>
                      {store.documents.other.products.params.map(({ title, value }, idx) => (
                        <tr key={idx}>
                          <th scope="row">{title}:</th>
                          <td>{value}</td>
                        </tr>
                      ))}
                    </tbody>
                  </Table>
                )}

                {store.documents.other.products.arguments.length > 0 && (
                  <Box backgroundColor="blue-green-light">
                    <Row
                      as="ul"
                      className="justify-content-center list-unstyled mb-0"
                      aria-label={t('productBenefits')}
                    >
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
                <div
                  className="text-center editorial-content my-4"
                  dangerouslySetInnerHTML={{ __html: store.documents.other.products.text }}
                />
                <div className="mb-2">
                  <Button
                    variant="link"
                    onClick={() => store.acceptAllDocuments(store.documentsProducts)}
                    aria-pressed={store.documentsProducts.every(d => d.accepted)}
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
                      <span className="mr-1">{prefix}</span>
                      <DocumentLink
                        url={`${getFileUrl}/${key}`}
                        label={label}
                        onClick={handleDownload}
                      />
                    </Form.Check.Label>
                  </FormCheckWrapper>
                ))}
                {/* info text */}
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
              <div
                className="editorial-content mb-3"
                dangerouslySetInnerHTML={{ __html: t('acceptOfferHelptext') }}
              />
              <Button
                variant="secondary"
                type="submit"
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
          thumbnailUrl={thumbnailUrl ?? ''}
          signFileUrl={signFileUrl ?? ''}
        />

        <ConfirmationModal
          show={confirmationModal}
          onClose={() => setConfirmationModal(false)}
          labels={labels}
          thankYouPageUrl={thankYouPageUrl}
        />
      </OfferStoreContext.Provider>
    )
  },
)
