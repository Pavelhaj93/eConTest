import { Icon, CallMeBackModal } from '@components'
import { useLabels } from '@hooks'
import { CommodityProductType, ResponseItemType, View } from '@types'
import classNames from 'classnames'
import { observer } from 'mobx-react-lite'
import React, { useEffect, useState } from 'react'
import { Alert, Button, Col, Container, Modal, Row } from 'react-bootstrap'
import { CallMeBackStore, SummaryStore } from '@stores'
import { CallMeBackStoreContext } from '@context'
import { removeLastElement } from '@utils'
import { colors } from '@theme'
import { closestIndexTo } from 'date-fns'
import ContractualData from '../blocks/ContractualData'
import Product from '../blocks/Product'
import Benefit from '../blocks/Benefit'
import GiftBlock from '../blocks/GiftBlock'
import DistributorChange from '../blocks/DistributorChange'

export const Summary: React.FC<View> = observer(
  ({
    getSummaryUrl,
    errorPageUrl,
    offerUrl,
    authUrl,
    labels,
    timeout,
    unfinishedOfferTimeout,
    guid,
    getCallMeBackUrl,
    postCallMeBackUrl,
    showCmbModal,
  }) => {
    const t = useLabels(labels)
    /* Summary store. */
    const [store] = useState<SummaryStore>(
      // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
      () => new SummaryStore(guid, getSummaryUrl!, errorPageUrl),
    )
    /* CallMeBack store. */
    const [storeCallMeBack] = useState<CallMeBackStore>(
      // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
      () => new CallMeBackStore(guid, getCallMeBackUrl!),
    )

    const [unfinishedOfferModalOpen, setUnfinishedOfferModalOpen] = useState<boolean>(false)
    const [callMeBackModalOpen, setCallMeBackModalOpen] = useState<boolean>(false)

    useEffect(() => {
      store.fetchSummary(timeout)
      // Uncomment for opening unfinished offer modal after specified time
      if (unfinishedOfferTimeout) {
        const interval = setInterval(() => {
          setUnfinishedOfferModalOpen(true)
        }, unfinishedOfferTimeout)
        return () => {
          clearInterval(interval)
        }
      }
      // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    const handleClick = () => {
      storeCallMeBack.fetchCallMeBackData()
      setCallMeBackModalOpen(true)
    }

    return (
      <>
        {store.error && (
          <Alert variant="danger">
            <h3>{t('appUnavailableTitle')}</h3>
            <div
              className="editorial-content"
              dangerouslySetInnerHTML={{ __html: t('appUnavailableText') }}
            />
          </Alert>
        )}
        <div
          className={classNames({
            loading: store.isLoading,
            'd-none': store.error, // hide the whole box if there is an error
          })}
        >
          <>
            {store.data?.map(item => {
              const { type } = item
              if (type === ResponseItemType.ContractualData) {
                return (
                  <ContractualData
                    key={item.position}
                    headerTitle={item.header.title}
                    bodyPersonalData={item.body.personalData}
                    bodyAddresses={item.body.addresses}
                    bodyContacts={item.body.contacts}
                  />
                )
              }
              if (type === ResponseItemType.Product) {
                return (
                  <Product
                    key={item.position}
                    headerType={item.header.type}
                    headerTitle={item.header.title}
                    headerData={item.header.data}
                    bodyPrices={item.body.prices}
                    bodyInfos={item.body.infos}
                    infoHelp={item.body.infoHelp}
                    bodyPoints={item.body.points}
                  />
                )
              }
              if (type === ResponseItemType.Benefit) {
                return <Benefit key={item.position} benefitData={item} />
              }
              if (type === ResponseItemType.Gift) {
                return <GiftBlock key={item.position} giftData={item} />
              }
              if (type === ResponseItemType.Competitor) {
                return (
                  <DistributorChange
                    key={item.position}
                    headerTitle={item.header.title}
                    bodyName={item.body.name}
                    bodyText={item.body.text}
                  />
                )
              }
            })}

            {!store.isLoading && !store.error && (
              <>
                {/* Continue and CallMeBack buttons. */}
                <div className="d-flex flex-column flex-md-row flex-wrap  justify-content-center">
                  <Button variant="primary" href={offerUrl}>
                    {t('continueBtn')}
                  </Button>
                  {showCmbModal && (
                    <Button
                      className="mt-3 ml-md-3 mt-md-0"
                      variant="outline-dark"
                      onClick={handleClick}
                    >
                      {t('cmbBtn')}
                    </Button>
                  )}
                </div>
                {/* Continue and CallMeBack buttons. */}
                {/*Unfinished offer modal. */}
                <Modal
                  size="xl"
                  show={unfinishedOfferModalOpen}
                  onHide={() => setUnfinishedOfferModalOpen(false)}
                  dialogClassName="unfinished-offer-modal"
                >
                  <Modal.Header
                    closeButton
                    closeLabel={t('modalClose')}
                    style={{
                      borderBottom: '0 none',
                    }}
                  />

                  <Modal.Body style={{ textAlign: 'center' }}>
                    <Icon name="cross-circle" size={70} color="#eb4b0a"></Icon>
                    <h2 className="text-red mt-4">{t('unfinishedOfferSummary')}</h2>
                    <div
                      className="mb-4"
                      dangerouslySetInnerHTML={{ __html: t('unfinishedOfferTextSummary') }}
                    />
                    <div className="button-group d-flex flex-wrap justify-content-center">
                      <Button
                        onClick={() => {
                          setUnfinishedOfferModalOpen(false)
                        }}
                        variant="primary"
                      >
                        {t('continueInOfferSummary')}
                      </Button>
                      <Button href={authUrl} variant="outline-danger">
                        {t('quitOfferSummary')}
                      </Button>
                    </div>
                  </Modal.Body>
                </Modal>
                {/*Unfinished offer modal. */}
                {/*CallMeBack modal. */}
                <CallMeBackStoreContext.Provider value={storeCallMeBack}>
                  <CallMeBackModal
                    show={callMeBackModalOpen}
                    onClose={() => setCallMeBackModalOpen(false)}
                    postCallMeBackUrl={postCallMeBackUrl || ''}
                  />
                </CallMeBackStoreContext.Provider>

                {/*CallMeBack modal. */}
              </>
            )}
          </>
        </div>
      </>
    )
  },
)
