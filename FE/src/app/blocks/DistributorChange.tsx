import React, { FC } from 'react'
import { SummaryResponse } from '../types/Summary'
import { Col, Container, Row } from 'react-bootstrap'

interface DistributorChangeProps {
  headerTitle: SummaryResponse.ResponseItem['header']['title']
  bodyName: SummaryResponse.ResponseItem['body']['name']
  bodyText: SummaryResponse.ResponseItem['body']['text']
}

const DistributorChange: FC<DistributorChangeProps> = ({ headerTitle, bodyName, bodyText }) => {
  return (
    <Container className="mb-4">
      <Row>
        <Col className="m-auto px-0" xs={12} lg={10}>
          <h2 id="distributorChange" className="text-center">
            {headerTitle}
          </h2>
          <hr className="hr" />
          <h3 aria-labelledby="distributorChange" className="h4 text-center">
            {bodyName}
          </h3>
          {bodyText && <div dangerouslySetInnerHTML={{ __html: bodyText }} />}
        </Col>
      </Row>
    </Container>
  )
}

export default DistributorChange
