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

interface ProductProps {
  headerType: SummaryResponse.ResponseItem['header']['type']
  headerTitle: SummaryResponse.ResponseItem['header']['title']
  headerData: SummaryResponse.ResponseItem['header']['data']
  bodyPrices: SummaryResponse.ResponseItem['body']['prices']
  bodyInfos: SummaryResponse.ResponseItem['body']['infos']
  infoHelp: SummaryResponse.ResponseItem['body']['infoHelp']
  bodyPoints: SummaryResponse.ResponseItem['body']['points']
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
    if (infoHelp && bodyInfos) {
      setLastProductMiddleTextElement(bodyInfos[bodyInfos.length - 1].value)
    }
  }, [bodyInfos, infoHelp])

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
                {headerData?.map((price, index) => (
                  <div className="d-flex align-items-center" key={index}>
                    <div className="d-flex flex-column flex-md-row align-items-center mb-0 mr-2">
                      <span className="mr-md-2">{price.title}</span>
                      <span className="font-weight-bold">{price.value}</span>
                    </div>
                    {price.note && (
                      <Tooltip
                        tooltipClassName="mb-2"
                        iconColor={colors.white}
                        name="question-mark"
                        size={25}
                      >
                        {price.note}
                      </Tooltip>
                    )}
                  </div>
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
            <Col key={i} className="text-center text-sm" xs={12} md={12 / bodyPrices.length}>
              <p className="font-weight-bold">{item.unit}</p>
              <p className="text-xs line-through">{item.price2}</p>
              <p className="special font-weight-bold">
                <span className="text-m">{item.price}</span> {item.unit}
              </p>
            </Col>
          ))}
        </Row>
        {bodyInfos &&
          !infoHelp &&
          bodyInfos?.map((text, i) => (
            <div
              className="text-sm text-center mb-3 mt-0"
              key={i}
              dangerouslySetInnerHTML={{ __html: text.value }}
            />
          ))}
        {infoHelp && bodyInfos && bodyInfos?.length > 0 && (
          <>
            <div
              className="text-sm text-center mb-3 mt-0"
              dangerouslySetInnerHTML={{ __html: bodyInfos[0].value }}
            />
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
          </>
        )}
        <Row className="m-0 justify-content-center">
          {bodyPoints?.map((text, i) => (
            <Col
              className={classNames({
                'p-0 d-flex xl-justify-content-center align-items-center mb-3': true,
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
