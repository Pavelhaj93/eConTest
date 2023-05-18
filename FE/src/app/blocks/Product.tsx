import React, { FC } from 'react'
import { SummaryResponse } from '../types/Summary'
import { Col, Row } from 'react-bootstrap'
import { Box } from '../components/Box'
import { Icon } from '../components/Icon'
import classNames from 'classnames'
import { CommodityProductType } from '@types'
import { Tooltip } from '../components/Tooltip'
import { colors } from '../theme/variables'
import { Accordion } from '../components/Accordion'

interface ProductProps {
  productData: SummaryResponse.ResponseItem
}

const Product: FC<ProductProps> = ({ productData }) => {
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
    <Accordion
      isOpen={true}
      header={
        <Box
          backgroundColor={
            productData.header.type === CommodityProductType.GAS ? 'blue' : 'purple-light'
          }
          className="accordion__button"
        >
          <div className="d-flex justify-content-between align-items-center">
            <h2 className="m-0">{productData.header.title}</h2>
            <div className="d-flex align-items-center">
              <div className="d-flex flex-column align-items-center text-center mr-3">
                {productData.header.data?.map(price => (
                  <>{renderPriceWithToolTip(price.value, price.title, price.value)}</>
                ))}
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
          'accordion__panel--blue': productData.header.type === CommodityProductType.GAS,
        })}
      >
        <Row className="justify-content-center flex-row">
          {productData?.body?.prices?.map((item, i) => (
            <Col
              key={i}
              className="text-center text-sm"
              xs={12}
              // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
              // TODO: Fix this
              md={12 / productData.body.prices.length}
            >
              <p className="font-weight-bold">{item.unit}</p>
              <p className="text-xs line-through">{item.price2}</p>
              <p className="special font-weight-bold">
                <span className="text-m">{item.price}</span> {item.unit}
              </p>
            </Col>
          ))}
        </Row>
        {productData?.body?.infos?.map((text, i) => (
          <div
            className="text-sm text-center mb-3 mt-0"
            key={i}
            dangerouslySetInnerHTML={{ __html: text.value }}
          />
        ))}
        {/* // TODO: zjistit proc chybi middle text helps */}
        {/* {store?.product?.middle_texts_help && store.product.middle_texts.length > 0 && (
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
    )} */}
        <Row className="m-0 justify-content-center">
          {productData?.body?.points?.map((text, i) => (
            <Col
              className={classNames({
                'p-0 d-flex xl-justify-content-center align-items-center mb-3': true,
                // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
                // TODO: Fix this
                'pr-3': i < productData.body.points.length - 1,
              })}
              xl={4}
              key={i}
            >
              <span className="special">
                <Icon name={'check-circle'} size={48} color="currentColor"></Icon>
              </span>
              <div
                className="text-sm text-left ml-3 mb-0"
                dangerouslySetInnerHTML={{ __html: text.value }}
              />
            </Col>
          ))}
        </Row>
      </div>
    </Accordion>
  )
}

export default Product
