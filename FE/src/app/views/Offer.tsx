import React, {
  useEffect,
  useState,
  useCallback,
  useRef,
  FormEvent,
  useMemo,
  Fragment,
} from 'react'
import { CommodityProductType, NewOfferResponseCopy, OfferType, View } from '@types'
import { observer } from 'mobx-react-lite'
import { OfferStore } from '@stores'
import { Alert, Button, Col, Form, Row, Table } from 'react-bootstrap'
import classNames from 'classnames'
import Media from 'react-media'
import {
  Box,
  BoxHeading,
  ConfirmationModal,
  UnfinishedOfferModal,
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
import { useKeepAlive, useLabels, useUnload } from '@hooks'
import { OfferStoreContext } from '@context'
import { isIE11, parseUrl } from '@utils'
import Perex from '../blocks/Perex'
import Benefit from '../blocks/Benefit'

type SignatureModalType = {
  id: string
  show: boolean
}

export const Offer: React.FC<View> = observer(
  ({
    guid,
    offerUrl,
    cancelDialog,
    labels,
    keepAliveUrl,
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
    backToOfferUrl,
    suppliers,
    version = 3,
  }) => {
    const [store] = useState(() => new OfferStore(OfferType.NEW, offerUrl, guid))
    const [signatureModalProps, setSignatureModalProps] = useState<SignatureModalType>({
      id: '',
      show: false,
    })
    const [confirmationModal, setConfirmationModal] = useState<boolean>(false)
    const t = useLabels(labels)
    const formRef = useRef<HTMLFormElement>(null)

    // keep session alive
    useKeepAlive(30 * 1000, keepAliveUrl ? parseUrl(keepAliveUrl, { guid }) : keepAliveUrl)

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

      if (cancelDialog) {
        store.cancelOfferUrl = cancelDialog.cancelOfferUrl
      }

      if (suppliers) {
        store.isSupplierMandatory = true
      }
      // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    // show warning to user when trying to refresh or leave the page once he did some changes
    useUnload(ev => {
      if (
        store.isOfferDirty &&
        !store.isAccepting &&
        !store.forceReload &&
        !store.isUnfinishedOfferModalOpen
      ) {
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

    const renderInfoElement = (value: string | undefined, className?: string) =>
      value && (
        <div className={classNames('text-center mt-4', className)}>
          <Icon
            name="info-circle"
            size={40}
            color={colors.gray100}
            className="d-block mx-auto mb-3"
          />
          <div className="editorial-content" dangerouslySetInnerHTML={{ __html: value }} />
        </div>
      )

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
          {store.concatedSortedData?.map(item => {
            const { type } = item
            if (type === NewOfferResponseCopy.ResponseItemType.Perex) {
              return (
                <Perex
                  key={item.position}
                  headerTitle={item.header.title}
                  bodyParams={item.body.params}
                />
              )
            }
            if (type === NewOfferResponseCopy.ResponseItemType.Benefit) {
              return (
                <Benefit
                  key={item.position}
                  headerTitle={item.header.title}
                  body={item.body}
                  bodyPoints={item.body.points}
                />
              )
            }
            if (type === NewOfferResponseCopy.ResponseItemType.DocsCheck) {
              return (
                <Fragment key={item.position}>
                  <Box>
                    <BoxHeading>{item.body.head?.title || item.header.title}</BoxHeading>
                    {item.body.head?.params && (
                      <div
                        className="editorial-content text-center"
                        dangerouslySetInnerHTML={{
                          __html: item.body.head.params
                            .map(param => param.title + ' ' + param.value)
                            .join(' '),
                        }}
                      />
                    )}
                  </Box>
                  {/* TODO: bylo tu jeste store.documents.description  zkontrolovat jestli to nechybi */}
                  {item.body.head?.text && (
                    <div
                      className="py-1 px-3 editorial-content text-center mb-4"
                      dangerouslySetInnerHTML={{
                        __html: item.body.head.text || '',
                      }}
                    />
                  )}
                  <Box data-testid="boxDocumentsToBeAccepted">
                    {store.docGroupsToBeChecked.length > 0 && (
                      <>
                        <BoxHeading>{item.body.docs?.title}</BoxHeading>
                        <div
                          className="my-4 text-center editorial-content"
                          dangerouslySetInnerHTML={{
                            __html: item.body.docs?.text ?? '',
                          }}
                        />
                        <div className="mb-2">
                          <Button
                            variant="link"
                            onClick={() => store.checkDocumentsGroup(item?.body?.docs?.files ?? [])}
                            aria-pressed={store.allDocumentsAreChecked}
                          >
                            {t('acceptAll')}
                          </Button>
                        </div>
                        {item.body.docs?.files.map(({ key, prefix, accepted, label, note }) => (
                          <>
                            <FormCheckWrapper
                              key={key}
                              type="checkbox"
                              name="acceptedDocuments"
                              id={`document-${key}`}
                              value={key}
                              checked={accepted}
                              onChange={() => store.checkDocument(key)}
                            >
                              <Form.Check.Label>
                                <span className="mr-1">{prefix}</span>
                                <DocumentLink
                                  url={parseUrl(`${getFileUrl}/${key}`, { guid })}
                                  label={label}
                                  onClick={handleDownload}
                                />
                              </Form.Check.Label>
                            </FormCheckWrapper>
                            {/* info text */}
                            {note && renderInfoElement(note, 'mb-5')}
                          </>
                        ))}
                      </>
                    )}
                  </Box>
                </Fragment>
              )
            }
            if (type === NewOfferResponseCopy.ResponseItemType.DocsSign) {
              return (
                <Fragment key={item.position}>
                  {store.docGroupsToBeSigned.length > 0 && (
                    <>
                      <BoxHeading>{item.header.title}</BoxHeading>
                      <div
                        className="editorial-content text-center my-4"
                        dangerouslySetInnerHTML={{
                          __html: item.body.docs?.text ?? '',
                        }}
                      />
                      {item.body.docs?.files.map(({ key, prefix, label, accepted, note }) => (
                        <>
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
                                  url={parseUrl(`${getFileUrl}/${key}?t=${new Date().getTime()}`, {
                                    guid,
                                  })}
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
                          {/* info text */}
                          {note && renderInfoElement(note, 'mb-4')}
                        </>
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
                </Fragment>
              )
            }
            if (type === NewOfferResponseCopy.ResponseItemType.Confirm) {
              return (
                <div
                  key={item.position}
                  className={classNames({
                    'd-none': (!store.isLoading && store.error) || !store.offerFetched,
                  })}
                >
                  <h2 className="mt-5 text-center">{t('acceptOfferTitle')}</h2>
                  <Box>
                    {suppliers && (
                      <Form.Group>
                        <Form.Label htmlFor="supplier">{suppliers.label}</Form.Label>
                        <Form.Control
                          as="select"
                          id="supplier"
                          value={store.supplier}
                          onChange={event => store.selectSupplier(event.target.value)}
                        >
                          {[{ label: '', value: '' }, ...suppliers.items].map(
                            ({ label, value }, idx) => (
                              <option key={idx} value={value}>
                                {label}
                              </option>
                            ),
                          )}
                        </Form.Control>
                      </Form.Group>
                    )}
                    <div className="text-center">
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
                      {/* If a user wants to start again, the following button will be visible */}
                      {cancelDialog && (
                        <Button
                          className="ml-3"
                          variant="outline-primary"
                          type="submit"
                          onClick={() => store.setIsUnfinishedOfferModalOpen(true)}
                        >
                          {t('startOver')}
                        </Button>
                      )}
                    </div>
                  </Box>
                </div>
              )
            }
          })}
        </form>
        {backToOfferUrl && (
          <Button
            variant="link"
            className="underline text-black m-auto w-content d-flex"
            href={backToOfferUrl}
          >
            {t('backToOffer', 'Zpět na nabídku')}
          </Button>
        )}
        <SignatureModal
          {...signatureModalProps}
          onClose={() => setSignatureModalProps({ show: false, id: '' })}
          labels={labels}
          thumbnailUrl={thumbnailUrl ?? ''}
          signFileUrl={signFileUrl ?? ''}
          guid={guid}
        />

        <ConfirmationModal
          show={confirmationModal && !store.isUnfinishedOfferModalOpen}
          onClose={() => setConfirmationModal(false)}
          labels={labels}
          thankYouPageUrl={thankYouPageUrl}
          cancelDialog={!!cancelDialog}
          openUnfinishedModal={() => store.setIsUnfinishedOfferModalOpen(true)}
        />

        <UnfinishedOfferModal
          show={store.isUnfinishedOfferModalOpen}
          onClose={() => store.setIsUnfinishedOfferModalOpen(false)}
          redirectUrl={cancelDialog?.redirectUrl ?? ''}
          labels={labels}
        />
      </OfferStoreContext.Provider>
    )
  },
)
