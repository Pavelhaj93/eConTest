import React, { useEffect, useState, useCallback, useRef, FormEvent } from 'react'
import { CommodityProductType, NewOfferResponse, OfferType, View } from '@types'
import { observer } from 'mobx-react-lite'
import { OfferStore } from '@stores'
import { Alert, Button } from 'react-bootstrap'
import classNames from 'classnames'

import { ConfirmationModal, UnfinishedOfferModal, SignatureModal } from '@components'

import { useKeepAlive, useLabels } from '@hooks'
import { OfferStoreContext } from '@context'
import { isIE11, parseUrl } from '@utils'

import DocsCheck from '../blocks/DocsCheck'
import DocsSign from '../blocks/DocsSign'
import Confirm from '../blocks/Confirm'

export const OfferEle: React.FC<View> = observer(
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
    errorPageUrl,
    acceptOfferUrl,
    sessionExpiredPageUrl,
    suppliers,
    backUrl,
    nextUrl,
  }) => {
    const [store] = useState(() => new OfferStore(OfferType.NEW, offerUrl, guid))
    const [openSignatureModal, setOpenSignatureModal] = useState<string | null>(null)
    const [confirmationModal, setConfirmationModal] = useState<boolean>(false)
    const t = useLabels(labels)
    const formRef = useRef<HTMLFormElement>(null)

    // keep session alive
    useKeepAlive(30 * 1000, keepAliveUrl ? parseUrl(keepAliveUrl, { guid }) : keepAliveUrl)

    useEffect(() => {
      store.errorPageUrl = errorPageUrl
      store.sessionExpiredPageUrl = sessionExpiredPageUrl
      store.fetchOffer(timeout)

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
    // useUnload(ev => {
    //   if (
    //     store.isOfferDirty &&
    //     !store.isAccepting &&
    //     !store.forceReload &&
    //     !store.isUnfinishedOfferModalOpen
    //   ) {
    //     ev.preventDefault()
    //     ev.returnValue = ''
    //   }
    // })

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
          {store.concatedSortedData?.map(item => {
            const { type } = item
            switch (type) {
              case NewOfferResponse.ResponseItemType.DocsCheckE: {
                return (
                  <DocsCheck
                    key={item.position}
                    t={t}
                    type={CommodityProductType.ELECTRICITY}
                    headerTitle={item.header.title}
                    headTitle={item.body.head?.title ?? ''}
                    headText={item.body.head?.text ?? ''}
                    bodyText={item.body.text ?? ''}
                    docsPerex={item.body.docs.perex}
                    docsTitle={item.body.docs?.title ?? ''}
                    docsFiles={item.body.docs?.files ?? []}
                    docsText={item.body.docs?.text ?? ''}
                    docsParams={item.body.docs?.params ?? []}
                    bodyNote={item.body.note ?? ''}
                    handleDownload={handleDownload}
                    getFileUrl={getFileUrl ?? ''}
                    guid={guid}
                  />
                )
              }
              case NewOfferResponse.ResponseItemType.DocsCheckG: {
                return (
                  <DocsCheck
                    key={item.position}
                    t={t}
                    type={CommodityProductType.GAS}
                    headerTitle={item.header.title}
                    headTitle={item.body.head?.title ?? ''}
                    headText={item.body.head?.text ?? ''}
                    bodyText={item.body.text ?? ''}
                    docsPerex={item.body.docs.perex}
                    docsTitle={item.body.docs?.title ?? ''}
                    docsFiles={item.body.docs?.files ?? []}
                    docsText={item.body.docs?.text ?? ''}
                    docsParams={item.body.docs?.params ?? []}
                    bodyNote={item.body.note ?? ''}
                    handleDownload={handleDownload}
                    getFileUrl={getFileUrl ?? ''}
                    guid={guid}
                  />
                )
              }
              case NewOfferResponse.ResponseItemType.DocsCheck: {
                return (
                  <DocsCheck
                    key={item.position}
                    t={t}
                    headerTitle={item.header.title}
                    headTitle={item.body.head?.title ?? ''}
                    headText={item.body.head?.text ?? ''}
                    bodyText={item.body.text ?? ''}
                    docsTitle={item.body.docs?.title ?? ''}
                    docsFiles={item.body.docs?.files ?? []}
                    docsText={item.body.docs?.text ?? ''}
                    docsParams={item.body.docs?.params ?? []}
                    bodyNote={item.body.note ?? ''}
                    handleDownload={handleDownload}
                    getFileUrl={getFileUrl ?? ''}
                    guid={guid}
                  />
                )
              }
              case NewOfferResponse.ResponseItemType.DocsSign: {
                return (
                  <DocsSign
                    key={item.position}
                    t={t}
                    headerTitle={item.header.title}
                    docsTitle={item.body.docs?.title ?? ''}
                    docsFiles={item.body.docs?.files ?? []}
                    getFileUrl={getFileUrl ?? ''}
                    bodyNote={item.body.note ?? ''}
                    guid={guid}
                    handleDownload={handleDownload}
                    onOpenSignatureModal={setOpenSignatureModal}
                  />
                )
              }
              case NewOfferResponse.ResponseItemType.Confirm: {
                return (
                  <Confirm
                    key={item.position}
                    t={t}
                    headerTitle={item.header.title ?? ''}
                    bodyText={item.body.text ?? ''}
                    suppliers={suppliers}
                    setConfirmationModal={setConfirmationModal}
                    cancelDialog={cancelDialog}
                  />
                )
              }
            }
          })}
        </form>

        {backUrl && (
          <Button
            variant="link"
            className="underline text-primary m-auto w-content d-flex"
            href={backUrl}
          >
            {t('backLabel', 'Zpět na nahrání dokumentů')}
          </Button>
        )}
        <SignatureModal
          id={openSignatureModal ?? ''}
          show={!!openSignatureModal}
          onClose={() => setOpenSignatureModal(null)}
          labels={labels}
          thumbnailUrl={thumbnailUrl ?? ''}
          signFileUrl={signFileUrl ?? ''}
          guid={guid}
        />

        <ConfirmationModal
          show={confirmationModal && !store.isUnfinishedOfferModalOpen}
          onClose={() => setConfirmationModal(false)}
          labels={labels}
          nextUrl={nextUrl ?? ''}
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
