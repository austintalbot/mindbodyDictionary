// admin-app/src/types/index.ts

export enum RecommendationType {
  Product = 0,
  Book = 2,
  Food = 3,
}

export interface Recommendation {
  name?: string;
  url?: string;
  recommendationType: RecommendationType;
}

export interface Image {
  name: string;
  uri?: string;
  mbdCondition?: string;
}

export interface Faq {
  id?: string;
  question: string;
  shortAnswer?: string;
  answer: string;
  order?: number;
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
}


// {
//   "id": "816ff179-67cf-4e3b-88ef-ab5dd230e13d",
//   "name": "Lung Problems",
//   "imageNegative": "lungProblemsNegative.png",
//   "imagePositive": "lungProblemsPositive.png",
//   "summaryNegative": "\"I'm so sad I can barely breathe.\" \nYou are experiencing panic or anxiety stemming from a sense of abandonment. \nThe grief, shame, loneliness, and hopeless have become overwhelming. \nYou may feel confused as you struggle to piece together how to honor your path and purpose, feeling it is against the opinions of others. \nYou've suppressed yourself, and you may feel smothered by how others think you should do things.\nStarved for love, you've been unwilling to maintain yourself from one moment to the next, believing you don't even deserve it. ",
//   "summaryPositive": "Observe your experience as separate from the essence of who you are to make space for healing. Breathe in and out as you use the affirmations to help bring vitality and enthusiasm back into your experience. When flying in an airplane, you have to put the breathing mask on yourself before you turn to help others. Be there for you, rise into you, then you will be able to help others appropriately. ",
//   "affirmations": [
//     "I desire. ",
//     "I expand into life.",
//     "I breathe in the gifts of God now.",
//     "I am alive and share.",
//     "I am open.",
//     "I breathe in life abundantly.",
//     "I acknowledge God in all things.",
//     "Everyone is my teacher.",
//     "I reflect with understanding in peace.",
//     "I am thoughtful."
//   ],
//   "physicalConnections": [
//     "Kidney",
//     "Sadness",
//     "Lungs",
//     "Lung"
//   ],
//   "searchTags": [
//     "smothered",
//     "Panic",
//     "Abandonment",
//     "Shame",
//     "Loneliness",
//     "hopeless",
//     "overwhelming",
//     "self-esteem",
//     "Lung",
//     "Lungs"
//   ],
//   "tags": null,
//   "recommendations": [
//     {
//       "name": "Apple   ",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Citrus",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Pear",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Peach",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Papaya",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Chickweed (for some) ",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Fennel",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Kelp",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Carrots ",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Greens",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Garlic",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Onions",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Persimmon",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Cauliflower",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Celery",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Chard",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Fenugreek seeds",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Fish",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Pumpkin",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Radish",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Seaweed",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Tomato",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Turkey (for some)",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Yam",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Possibly brown rice",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Cantaloupe (for some - if candida or fungus do not use)",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Use garlic & onion to sniff when there is lung congestion.",
//       "url": "",
//       "recommendationType": 3
//     },
//     {
//       "name": "Life Centering with Breath & Awareness: A Step-by-Step Guide to Self-Empowerment and Transformation",
//       "url": "https://amzn.to/2SfEXuv",
//       "recommendationType": 2
//     },
//     {
//       "name": "Mindbody Movements",
//       "url": "https://mindbodymovements.com/",
//       "recommendationType": 2
//     },
//     {
//       "name": "Insight Timer",
//       "url": "https://play.google.com/store/apps/details?id=com.spotlightsix.zentimerlite2",
//       "recommendationType": 2
//     },
//     {
//       "name": "Insight Journal: Personal Exploration  ",
//       "url": "https://amzn.to/42ot9vK",
//       "recommendationType": 2
//     },
//     {
//       "name": "Mindbody Dictionary Workbook ",
//       "url": "https://amzn.to/3D0OUHG",
//       "recommendationType": 2
//     },
//     {
//       "name": "Sacred Contracts",
//       "url": "https://amzn.to/2OsLIbo",
//       "recommendationType": 2
//     },
//     {
//       "name": "Brigham Tea Herb",
//       "url": "https://amzn.to/4hWw2b6",
//       "recommendationType": 0
//     },
//     {
//       "name": "Bioflavonoid Complex",
//       "url": "https://amzn.to/40lJY82",
//       "recommendationType": 0
//     },
//     {
//       "name": "Algae Calcium Supplement",
//       "url": "https://amzn.to/41YireI",
//       "recommendationType": 0
//     },
//     {
//       "name": "Cayenne Pepper Health Support Blend",
//       "url": "https://amzn.to/4h8xms5",
//       "recommendationType": 0
//     },
//     {
//       "name": "Pure Encapsulations Choline",
//       "url": "https://amzn.to/42ayCo8",
//       "recommendationType": 0
//     },
//     {
//       "name": "Silver Biotics Daily Immune Support Supplement  ",
//       "url": "https://amzn.to/3VVeXqF",
//       "recommendationType": 0
//     },
//     {
//       "name": "Pure Encapsulations CoQ10",
//       "url": "https://amzn.to/4lh9dlh",
//       "recommendationType": 0
//     },
//     {
//       "name": "Digestive Enzyme Supplement",
//       "url": "https://amzn.to/4aoIWMX",
//       "recommendationType": 0
//     },
//     {
//       "name": "Elecampane for Respiratory System Support",
//       "url": "https://amzn.to/42gexfY",
//       "recommendationType": 0
//     },
//     {
//       "name": "Essential Fatty Acids",
//       "url": "https://amzn.to/3C3VX1P",
//       "recommendationType": 0
//     },
//     {
//       "name": "Omega-3 Fish Oil Supplement",
//       "url": "https://amzn.to/3FZU7QU",
//       "recommendationType": 0
//     },
//     {
//       "name": "Grape Seed Extract",
//       "url": "https://amzn.to/4g6CRG0",
//       "recommendationType": 0
//     },
//     {
//       "name": "Irish Sea Moss",
//       "url": "https://amzn.to/42bMb8g",
//       "recommendationType": 0
//     },
//     {
//       "name": "O.N.E. Multivitamin ",
//       "url": "https://amzn.to/4jk0mP1",
//       "recommendationType": 0
//     },
//     {
//       "name": "Sacred 7 Mushroom Extract Powder ",
//       "url": "https://amzn.to/3XSWWJ2",
//       "recommendationType": 0
//     },
//     {
//       "name": "NAC Supplement for Lung Health",
//       "url": "https://amzn.to/4l9ZIUZ",
//       "recommendationType": 0
//     },
//     {
//       "name": "L-Methionine Supplement",
//       "url": "https://amzn.to/42elIFL",
//       "recommendationType": 0
//     },
//     {
//       "name": "L-Taurine Amino Acid Supplement ",
//       "url": "https://amzn.to/3R1RjVW",
//       "recommendationType": 0
//     },
//     {
//       "name": "Solomon's Seal Root",
//       "url": "https://amzn.to/3R1h20D",
//       "recommendationType": 0
//     },
//     {
//       "name": "Spirulina Tablets",
//       "url": "https://amzn.to/3BRso3l",
//       "recommendationType": 0
//     },
//     {
//       "name": "Nutritional Yeast Seasoning ",
//       "url": "https://amzn.to/4gTo1oG",
//       "recommendationType": 0
//     }
//   ],
//   "subscriptionOnly": true
// }
