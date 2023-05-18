import React, { FC } from 'react'
import { Box, BoxHeading } from '../components/Box'
import { Col, Row } from 'react-bootstrap'
import { SummaryResponse } from '../types/Summary'
import { Gift } from '../components/Gift'

interface GiftBlockProps {
  headerTitle: SummaryResponse.ResponseItem['header']['title']
  bodyGroups: SummaryResponse.ResponseItem['body']['groups']
  headerNote?: SummaryResponse.ResponseItem['header']['note']
}

const GiftBlock: FC<GiftBlockProps> = ({ headerTitle, bodyGroups, headerNote }) => {
  return (
    <Box backgroundColor="green">
      <BoxHeading data-testid="giftBoxHeading">{headerTitle}</BoxHeading>

      <Row className="justify-content-xl-center">
        {bodyGroups?.map(({ title, params }, idx) => (
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
                      className={(bodyGroups?.length === 1 && 'justify-content-xl-center') || ''}
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

      {headerNote && (
        <div
          className="text-center editorial-content"
          dangerouslySetInnerHTML={{ __html: headerNote }}
        />
      )}
    </Box>
  )
}

export default GiftBlock
