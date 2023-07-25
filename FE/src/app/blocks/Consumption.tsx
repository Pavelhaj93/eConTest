import React, { FC } from 'react'
import { Box } from '../components/Box'
import { NewOfferResponse } from '../types/Offer'

interface ConsumptionProps {
  headTitle: NewOfferResponse.Header['title']
  headText: NewOfferResponse.Header['text']
}

const Consumption: FC<ConsumptionProps> = ({ headTitle, headText }) => {
  return (
    <Box className="mb-4">
      <h3 className="text-center font-weight-bolder">{headTitle}</h3>

      <div
        className="editorial-content text-center font-weight-bold "
        dangerouslySetInnerHTML={{
          __html: headText,
        }}
      />
    </Box>
  )
}

export default Consumption
