import React, { useEffect, useState, useCallback, useRef, FormEvent } from 'react'
import { NewOfferResponseCopy, OfferType, View } from '@types'
import { observer } from 'mobx-react-lite'
import { OfferStore } from '@stores'
import { Alert, Button } from 'react-bootstrap'
import classNames from 'classnames'

import { ConfirmationModal, UnfinishedOfferModal, SignatureModal } from '@components'

import { useKeepAlive, useLabels, useUnload } from '@hooks'
import { OfferStoreContext } from '@context'
import { isIE11, parseUrl } from '@utils'
import Perex from '../blocks/Perex'
import Benefit from '../blocks/Benefit'
import DocsCheck from '../blocks/DocsCheck'
import DocsSign from '../blocks/DocsSign'
import Confirm from '../blocks/Confirm'

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
            switch (type) {
              case NewOfferResponseCopy.ResponseItemType.Perex: {
                return (
                  <Perex
                    key={item.position}
                    headerTitle={item.header.title}
                    bodyParams={item.body.params}
                  />
                )
              }
              case NewOfferResponseCopy.ResponseItemType.Benefit: {
                return (
                  <Benefit
                    key={item.position}
                    headerTitle={item.header.title}
                    body={item.body}
                    bodyPoints={item.body.points}
                  />
                )
              }
              case NewOfferResponseCopy.ResponseItemType.DocsCheck: {
                return (
                  <DocsCheck
                    key={item.position}
                    t={t}
                    headTitle={item.body.head?.title || item.header.title}
                    headParams={item.body.head?.params || []}
                    headText={item.body.head?.text || ''}
                    docGroupsToBeChecked={store.docGroupsToBeChecked}
                    allDocumentsAreChecked={store.allDocumentsAreChecked}
                    checkDocumentsGroup={store.checkDocumentsGroup}
                    checkDocument={store.checkDocument}
                    docsTitle={item.body.docs?.title || ''}
                    docsFiles={item.body.docs?.files || []}
                    docsText={item.body.docs?.text || ''}
                    handleDownload={handleDownload}
                    getFileUrl={getFileUrl}
                    guid={guid}
                  />
                )
              }
              case NewOfferResponseCopy.ResponseItemType.DocsSign: {
                return (
                  <DocsSign
                    t={t}
                    docGroupsToBeSigned={store.docGroupsToBeSigned}
                    headerTitle={item.header.title}
                    docsText={item.body.docs?.text || ''}
                    docsFiles={item.body.docs?.files || []}
                    getFileUrl={getFileUrl}
                    guid={guid}
                    handleDownload={handleDownload}
                    openSignatureModal={openSignatureModal}
                  />
                )
              }
              case NewOfferResponseCopy.ResponseItemType.Confirm: {
                return (
                  <Confirm
                    t={t}
                    isLoading={store.isLoading}
                    error={store.error}
                    offerFetched={store.offerFetched}
                    suppliers={suppliers}
                    supplier={store.supplier}
                    selectSupplier={store.selectSupplier}
                    setConfirmationModal={setConfirmationModal}
                    isOfferReadyToAccept={store.isOfferReadyToAccept}
                    setIsUnfinishedOfferModalOpen={store.setIsUnfinishedOfferModalOpen}
                    cancelDialog={cancelDialog}
                  />
                )
              }
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
