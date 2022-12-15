import { Accordion, Box, BoxHeading, Gift, Icon, CallMeBackModal, Tooltip } from '@components'
import { useLabels } from '@hooks'
import { View } from '@types'
import classNames from 'classnames'
import { observer } from 'mobx-react-lite'
import React, { useEffect, useState } from 'react'
import { Alert, Button, Col, Container, Modal, Row } from 'react-bootstrap'
import { CallMeBackStore, SummaryStore } from '@stores'
import { CallMeBackStoreContext } from '@context'
import { removeLastElement } from '@utils'
import { colors } from '@theme'

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
    const [lastProductMiddleTextElement, setLastProductMiddleTextElement] = useState<
      string | undefined
    >(undefined)

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

    useEffect(() => {
      if (store?.product?.middle_texts_help) {
        setLastProductMiddleTextElement(removeLastElement(store?.product?.middle_texts))
      }
    }, [store?.product?.middle_texts, store?.product?.middle_texts_help])

    const renderPriceWithToolTip = (price: string, priceDescription: string, priceNote: string) => (
      <div className="d-flex align-items-center">
        <div className="d-flex flex-column flex-md-row align-items-center mb-0 mr-2">
          <span className="mr-md-2">{priceDescription}</span>
          <span className="font-weight-bold">{price}</span>
        </div>
        {priceNote && (
          <Tooltip tooltipClassName="mb-2" iconColor={colors.white} name="question-mark" size={25}>
            {priceNote}
          </Tooltip>
        )}
      </div>
    )

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
            {/* Personal Details Box */}
            {store.personalDetails && (
              <Box backgroundColor="gray-80">
                <BoxHeading>{store.personalDetails.title}</BoxHeading>
                <Row className="justify-content-center mx-0 mb-0">
                  {store.personalDetails.data.map((col, idx) => (
                    <Col as="ul" key={idx} xs={12} xl={4} className="my-3 px-2 list-unstyled">
                      <li>{col.title}</li>
                      {col.lines.map((line, i) => (
                        <li className="font-weight-bold" key={i}>
                          {line}
                        </li>
                      ))}
                    </Col>
                  ))}
                </Row>
                {store.personalDetails.communication && (
                  <ul className="my-3 list-unstyled">
                    <li>{store.personalDetails.communication.Title}</li>
                    <li className="font-weight-bold">
                      {store.personalDetails.communication.Value}
                    </li>
                  </ul>
                )}
              </Box>
            )}
            {/* Personal Details Box */}
            {/*Product Accordion */}

            {store.product && (
              <Accordion
                isOpen={true}
                header={
                  <Box
                    backgroundColor={
                      store.product.type.toLowerCase() === 'g' ? 'blue' : 'purple-light'
                    }
                    className="accordion__button"
                  >
                    <div className="d-flex justify-content-between align-items-center">
                      <h2 className="m-0">{store.product.header.name}</h2>
                      <div className="d-flex align-items-center">
                        <div className="d-flex flex-column align-items-center text-center mr-3">
                          {renderPriceWithToolTip(
                            store.product.header.price1,
                            store.product.header.price1_description,
                            store.product.header.price1_note,
                          )}
                          {renderPriceWithToolTip(
                            store.product.header.price2,
                            store.product.header.price2_description,
                            store.product.header.price2_note,
                          )}
                        </div>
                        <Icon size={32} color="currentColor" name="chevron-down"></Icon>
                      </div>
                    </div>
                  </Box>
                }
              >
                <div
                  className={classNames({
                    accordion__panel: true,
                    'accordion__panel--blue': store.product.type.toLowerCase() === 'g',
                  })}
                >
                  <Row className="justify-content-center">
                    {store.product.info_prices.map((item, i) => (
                      <Col
                        key={i}
                        className="text-center text-sm"
                        xs={12}
                        // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
                        md={12 / store.product!.info_prices.length}
                      >
                        <p className="font-weight-bold">{item.title}</p>
                        <p className="text-xs line-through">{item.previous_price}</p>
                        <p className="special font-weight-bold">
                          <span className="text-m">{item.value}</span> {item.value_unit}
                        </p>
                      </Col>
                    ))}
                  </Row>
                  {store.product.middle_texts.map((text, i) => (
                    <div
                      className="text-sm text-center mb-3 mt-0"
                      key={i}
                      dangerouslySetInnerHTML={{ __html: text }}
                    />
                  ))}
                  {store?.product?.middle_texts_help && store.product.middle_texts.length > 0 && (
                    <div className="d-flex justify-content-center align-items-baseline">
                      {lastProductMiddleTextElement && (
                        <div
                          className="text-sm text-center mb-3 mt-0 mr-2"
                          dangerouslySetInnerHTML={{
                            __html: lastProductMiddleTextElement,
                          }}
                        />
                      )}
                      <Tooltip name="question-mark" size={25}>
                        {store?.product?.middle_texts_help}
                      </Tooltip>
                    </div>
                  )}
                  <Row className="m-0 justify-content-center">
                    {store.product.benefits.map((text, i) => (
                      <Col
                        className={classNames({
                          'p-0 d-flex xl-justify-content-center align-items-center mb-3': true,
                          // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
                          'pr-3': i < store.product!.benefits.length - 1,
                        })}
                        xl={4}
                        key={i}
                      >
                        <span className="special">
                          <Icon name={'check-circle'} size={48} color="currentColor"></Icon>
                        </span>
                        <div
                          className="text-sm text-left ml-3 mb-0"
                          dangerouslySetInnerHTML={{ __html: text }}
                        />
                      </Col>
                    ))}
                  </Row>
                </div>
              </Accordion>
            )}
            {/*Product Accordion */}
            {/* Benefits accordion */}
            {store.benefits &&
              store.benefits.map((benefit, index) => (
                <div className="mb-4" key={index}>
                  <Accordion
                    isOpen={true}
                    header={
                      <Box
                        backgroundColor="orange"
                        className="accordion__button font-weight-bold d-flex justify-content-between align-items-center"
                      >
                        <h3 className="mb-0 pe-none">{benefit.title}</h3>
                        <Icon
                          size={32}
                          color="currentColor"
                          name="chevron-down"
                          className="pe-none"
                        ></Icon>
                      </Box>
                    }
                  >
                    <div className="accordion__panel accordion__panel--orange">
                      {benefit.summary && (
                        <Row className="m-0">
                          <Col
                            xs={12}
                            className="accordion__divider d-flex flex-column align-items-center p-0 mb-4 pb-3"
                          >
                            {benefit.summary.length > 0 && (
                              <ul className="list-unstyled mb-0">
                                {benefit.summary.map(({ title, value }, idx) => (
                                  <li className="text-sm text-center mb-2" key={idx}>
                                    {title} {value}
                                  </li>
                                ))}
                              </ul>
                            )}
                          </Col>
                        </Row>
                      )}
                      <Row className="m-0 justify-content-center">
                        {benefit.params.map((param, i) => (
                          <Col
                            className={classNames({
                              'p-0 d-flex xl-justify-content-center align-items-center mb-3': true,
                              // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
                              'pr-3': i < benefit.params.length - 1,
                            })}
                            xl={4}
                            key={i}
                          >
                            <span className="special">
                              <Icon name={'check-circle'} size={48} color="currentColor"></Icon>
                            </span>
                            <div
                              dangerouslySetInnerHTML={{ __html: param.value }}
                              className="text-sm text-left ml-3 mb-0"
                            ></div>
                          </Col>
                        ))}
                      </Row>
                    </div>
                  </Accordion>
                </div>
              ))}

            {/* Benefits accordion */}
            {/* Distributr change */}
            {store.distributorChange && (
              <Container className="mb-4">
                <Row>
                  <Col className="m-auto px-0" xs={12} lg={10}>
                    <h2 id="distributorChange" className="text-center">
                      {store.distributorChange.title}
                    </h2>
                    <hr className="hr" />
                    <h3 aria-labelledby="distributorChange" className="h4 text-center">
                      {store.distributorChange.name}
                    </h3>
                    <div
                      dangerouslySetInnerHTML={{ __html: store.distributorChange.description }}
                    />
                  </Col>
                </Row>
              </Container>
            )}
            {/* Distributr change */}
            {/* gifts box */}
            {store.gifts && (
              <Box backgroundColor="green">
                <BoxHeading data-testid="giftBoxHeading">{store.gifts.title}</BoxHeading>

                <Row className="justify-content-xl-center">
                  {store.gifts.groups.map(({ title, params }, idx) => (
                    <Col
                      key={idx}
                      xs={12}
                      md={6}
                      // show a different number of columns based on groups length
                      xl={4}
                      className="d-flex flex-column mb-5"
                    >
                      <h4 className="mb-4 gift__group-title text-xl-center">{title}</h4>
                      {params.length > 0 && (
                        <ul className="list-unstyled mb-0">
                          {params.map(({ title, icon, count }, idx) => (
                            <li key={idx} className="mb-3">
                              <Gift
                                className={
                                  (store?.gifts?.groups?.length === 1 &&
                                    'justify-content-xl-center') ||
                                  ''
                                }
                                type={icon}
                                title={`${count}x ${title}`}
                              />
                            </li>
                          ))}
                        </ul>
                      )}
                    </Col>
                  ))}
                </Row>

                <div
                  className="text-center editorial-content"
                  dangerouslySetInnerHTML={{ __html: store.gifts.note }}
                />
              </Box>
            )}
            {/* gifts box */}
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
