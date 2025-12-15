// admin-app/src/types/index.ts

export interface Recommendation {
  name?: string;
  url?: string;
  recommendationType: number;
}

export interface MbdCondition {
  id?: string;
  name?: string;
  imageNegative?: string;
  imagePositive?: string;
  summaryNegative?: string;
  summaryPositive?: string;
  affirmations?: string[];
  physicalConnections?: string[];
  searchTags?: string[];
  tags?: string[];
  recommendations?: Recommendation[];
  subscriptionOnly: boolean;
  imageShareOverrideAilmentName?: string;
}
