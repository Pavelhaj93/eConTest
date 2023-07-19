import React, { FC } from 'react'
import { NewOfferResponse } from '../types/Offer'
import { Box, BoxHeading } from '../components/Box'
import { Table } from 'react-bootstrap'

interface PerexProps {
  headerTitle: NewOfferResponse.Header['title']
  bodyParams: NewOfferResponse.Body['params']
}

const Perex: FC<PerexProps> = ({ headerTitle, bodyParams }) => {
  return (
    <Box backgroundColor="gray-80" className="mb-2">
      <BoxHeading>{headerTitle}</BoxHeading>
      {bodyParams && bodyParams.length > 0 && (
        <Table size="sm" borderless data-testid="summaryTable" className="table-two-columns">
          <tbody>
            {bodyParams.map(({ title, value }, idx) => (
              <tr key={idx}>
                <th scope="row">{title}:</th>
                <td>{value}</td>
              </tr>
            ))}
          </tbody>
        </Table>
      )}
    </Box>
  )
}

export default Perex
