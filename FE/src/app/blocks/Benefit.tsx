import React, { FC } from 'react'
import { Accordion } from '../components/Accordion'
import { Box } from '../components/Box'
import { Icon } from '../components/Icon'
import { Col, Row } from 'react-bootstrap'
import classNames from 'classnames'
import { SummaryResponse } from '../types/Summary'

interface BenefitProps {
  benefitData: SummaryResponse.ResponseItem
}

const Benefit: FC<BenefitProps> = ({ benefitData }) => {
  return (
    <div className="mb-4">
      <Accordion
        isOpen={true}
        header={
          <Box
            backgroundColor="orange"
            className="accordion__button font-weight-bold d-flex justify-content-between align-items-center"
          >
            <h3 className="mb-0 pe-none">{benefitData.header.title}</h3>
            <Icon size={32} color="currentColor" name="chevron-down" className="pe-none"></Icon>
          </Box>
        }
      >
        <div className="accordion__panel accordion__panel--orange">
          {benefitData.body && (
            <Row className="m-0">
              <Col
                xs={12}
                className="accordion__divider d-flex flex-column align-items-center p-0 mb-4 pb-3"
              >
                {Object.values(benefitData.body).length > 0 && (
                  <ul className="list-unstyled mb-0">
                    {benefitData.body.infos?.map(({ title, value }, idx) => (
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
            {benefitData.body.points?.map((point, i) => (
              <Col
                className={classNames({
                  'p-0 d-flex xl-justify-content-center align-items-center mb-3': true,
                  // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
                  'pr-3': i < benefitData.body.points.length - 1,
                })}
                xl={4}
                key={i}
              >
                <span className="special">
                  <Icon name={'check-circle'} size={48} color="currentColor"></Icon>
                </span>
                <div
                  dangerouslySetInnerHTML={{ __html: point.value }}
                  className="text-sm text-left ml-3 mb-0"
                ></div>
              </Col>
            ))}
          </Row>
        </div>
      </Accordion>
    </div>
  )
}

export default Benefit
