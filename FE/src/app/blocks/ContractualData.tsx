import React, { FC } from 'react'
import { Box, BoxHeading } from '../components/Box'
import { Col, Row } from 'react-bootstrap'
import { SummaryResponse } from '@types'

interface ContractualDataProps {
  headerTitle: SummaryResponse.ResponseItem['header']['title']
  bodyPersonalData: SummaryResponse.ResponseItem['body']['personalData']
  bodyAddresses: SummaryResponse.ResponseItem['body']['addresses']
  bodyContacts: SummaryResponse.ResponseItem['body']['contacts']
}

const ContractualData: FC<ContractualDataProps> = ({
  headerTitle,
  bodyPersonalData,
  bodyAddresses,
  bodyContacts,
}) => {
  return (
    <Box backgroundColor="gray-80">
      <BoxHeading>{headerTitle}</BoxHeading>
      <Row className="justify-content-center mx-0 mb-0 flex-row">
        <Col as="ul" xs={12} xl={4} className="my-3 px-2 list-unstyled">
          {bodyPersonalData?.map((item, index) => (
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
        {bodyAddresses?.map((item, index) => (
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
      {bodyContacts && (
        <ul className="my-3 list-unstyled">
          {bodyContacts.map((item, index) => (
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
