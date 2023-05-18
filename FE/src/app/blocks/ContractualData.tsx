import React, { FC } from 'react'
import { Box, BoxHeading } from '../components/Box'
import { Col, Row } from 'react-bootstrap'
import { SummaryResponse } from '@types'

interface ContractualDataProps {
  contractualData: SummaryResponse.ResponseItem
}

const ContractualData: FC<ContractualDataProps> = ({ contractualData }) => {
  return (
    <Box backgroundColor="gray-80">
      <BoxHeading>{contractualData.header.title}</BoxHeading>
      <Row className="justify-content-center mx-0 mb-0 flex-row">
        <Col as="ul" xs={12} xl={4} className="my-3 px-2 list-unstyled">
          {contractualData?.body?.personalData?.map((item, index) => (
            <div key={index}>
              <li>{item.title}</li>
              {item.values.map((value, index) => (
                <li className="font-weight-bold" key={index}>
                  {value}
                </li>
              ))}
            </div>
          ))}
        </Col>
        {contractualData?.body?.addresses?.map((item, index) => (
          <Col key={index} as="ul" xs={12} xl={4} className="my-3 px-2 list-unstyled">
            <>
              <li>{item.title}</li>
              {item.values.map((value, index) => (
                <li className="font-weight-bold" key={index}>
                  {value}
                </li>
              ))}
            </>
          </Col>
        ))}
      </Row>
      {contractualData.body.contacts && (
        <ul className="my-3 list-unstyled">
          {contractualData.body.contacts.map((item, index) => (
            <div key={index}>
              <li>{item.title}</li>
              {item.values.map((value, index) => (
                <li key={index} className="font-weight-bold">
                  {value}
                </li>
              ))}
            </div>
          ))}
        </ul>
      )}
    </Box>
  )
}

export default ContractualData
