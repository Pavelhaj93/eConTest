import React, { FC, useEffect, useState } from 'react'
import { SummaryResponse } from '../types/Summary'
import { Col, Row } from 'react-bootstrap'
import { Box } from '../components/Box'
import { Icon } from '../components/Icon'
import classNames from 'classnames'
import { CommodityProductType } from '@types'
import { Tooltip } from '../components/Tooltip'
import { colors } from '../theme/variables'
import { Accordion } from '../components/Accordion'
import { removeLastElement } from '../utils/strings'
import { info } from 'console'

interface ProductProps {
  headerType: SummaryResponse.ResponseItem['header']['type'] | null
  headerTitle: SummaryResponse.ResponseItem['header']['title'] | null
  headerData: SummaryResponse.ResponseItem['header']['data'] | null
  bodyPrices: SummaryResponse.ResponseItem['body']['prices'] | null
  bodyInfos: SummaryResponse.ResponseItem['body']['infos'] | null
  infoHelp: SummaryResponse.ResponseItem['body']['infoHelp'] | null
  bodyPoints: SummaryResponse.ResponseItem['body']['points'] | null
}

const Product: FC<ProductProps> = ({
  headerType,
  headerTitle,
  headerData,
  bodyPrices,
  bodyInfos,
  infoHelp,
  bodyPoints,
}) => {
  const [lastProductMiddleTextElement, setLastProductMiddleTextElement] = useState<
    string | undefined
  >(undefined)

  useEffect(() => {
    if (infoHelp) {
      setLastProductMiddleTextElement(removeLastElement(bodyInfos?.map(info => info.value ?? [])))
    }
  }, [bodyInfos, infoHelp])

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
          backgroundColor={headerType === CommodityProductType.GAS ? 'blue' : 'purple-light'}
          className="accordion__button"
        >
          <div className="d-flex justify-content-between align-items-center">
            <h2 className="m-0">{headerTitle}</h2>
            <div className="d-flex align-items-center">
              <div className="d-flex flex-column align-items-center text-center mr-3">
                {headerData?.map(price => (
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
          'accordion__panel--blue': headerType === CommodityProductType.GAS,
        })}
      >
        <Row className="justify-content-center flex-row">
          {bodyPrices?.map((item, i) => (
            <Col
              key={i}
              className="text-center text-sm"
              xs={12}
              // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
              // TODO: Fix this
              md={12 / bodyPrices.length}
            >
              <p className="font-weight-bold">{item.unit}</p>
              <p className="text-xs line-through">{item.price2}</p>
              <p className="special font-weight-bold">
                <span className="text-m">{item.price}</span> {item.unit}
              </p>
            </Col>
          ))}
        </Row>
        {bodyInfos?.map((text, i) => (
          <div
            className="text-sm text-center mb-3 mt-0"
            key={i}
            dangerouslySetInnerHTML={{ __html: text.value }}
          />
        ))}
        {infoHelp && bodyInfos && bodyInfos?.length > 0 && (
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
              {infoHelp}
            </Tooltip>
          </div>
        )}
        <Row className="m-0 justify-content-center">
          {bodyPoints?.map((text, i) => (
            <Col
              className={classNames({
                'p-0 d-flex xl-justify-content-center align-items-center mb-3': true,
                // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
                // TODO: Fix this
                'pr-3': i < bodyPoints.length - 1,
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
