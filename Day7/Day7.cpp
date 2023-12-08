#include <iostream>
#include <vector>
#include <fstream>
#include <string>
#include <map>
#include <algorithm>
#include <functional>


using namespace std;

enum HandType
{
    HighCard,
    OnePair,
    TwoPair,
    ThreeOfAKind,
    FullHouse,
    FourOfAKind,
    FiveOfAKind
};

enum CardType : int
{
    Two = 2,
    Three,
    Four,
    Five, 
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King, 
    Ace 
};

const int kNumCardsInHand = 5;

struct Hand
{
    CardType cards[kNumCardsInHand];
    int bid;
    HandType handType;
};

// function prototypes
vector<Hand> GetHands(string filename);
CardType GetCardType(char c);
HandType GetHandType(CardType* cards, int numCards);
bool CompareHands(Hand hand1, Hand hand2);
void PrintHand(Hand hand);

int main()
{
    string filename = "PuzzleFile.txt";
    vector<Hand> hands = GetHands(filename);
    
    // sort the hands in order
    sort(hands.begin(), hands.end(), CompareHands); 
    
    // calculate the winnings
    int totalWinnings = 0;

    for (int i = 0; i < hands.size(); i++)
    {
        //PrintHand(hands[i]);

        int rank = i + 1;
        int winnings = rank * hands[i].bid;
        //cout << hands[i].bid << " * " << rank << " = " << winnings << endl;
        totalWinnings += winnings;
    }

    cout << "Total Winnings = " << totalWinnings << endl;
    return 0;
}

vector<Hand> GetHands(string filename)
{
    std::ifstream file(filename);
    vector<Hand> hands;
    std::string line;
    
    while (getline(file, line)) {

        // split the line to get the cards and bid as strings
        int deliminatorIndex = line.find(" ");
        string cardsAsString = line.substr(0, deliminatorIndex);
        string bidAsString = line.substr(deliminatorIndex + 1, line.size() - 1);

        if (cardsAsString.size() != kNumCardsInHand)
        {
            throw std::invalid_argument("Num cards invalid");
        }

        // create a Hand stuct
        Hand hand; 
        hand.bid = atoi(bidAsString.c_str()); // need to convert string to const char* using c_str()
        for (int i = 0; i < cardsAsString.size(); i++)
        {
            hand.cards[i] = GetCardType(cardsAsString[i]);
        }
        hand.handType = GetHandType(hand.cards, kNumCardsInHand);
        
        // add to vector
        hands.push_back(hand);
    }

    return hands;
}

CardType GetCardType(char c)
{
    if (c == '2') return CardType::Two;
    if (c == '3') return CardType::Three;
    if (c == '4') return CardType::Four;
    if (c == '5') return CardType::Five;
    if (c == '6') return CardType::Six;
    if (c == '7') return CardType::Seven;
    if (c == '8') return CardType::Eight;
    if (c == '9') return CardType::Nine;
    if (c == 'T') return CardType::Ten;
    if (c == 'J') return CardType::Jack;
    if (c == 'Q') return CardType::Queen;
    if (c == 'K') return CardType::King;
    if (c == 'A') return CardType::Ace;

    throw std::invalid_argument("card is not an expected card type");
}

HandType GetHandType(CardType* cards, int numCards)
{
    // sort cards into a map with number of cards for each card type
    std::map<CardType, int> cardMap;
    for (int i = 0; i < numCards; i++)
    {
        CardType card = cards[i];
        auto search = cardMap.find(card);

        if (search != cardMap.end())
        {
            // exists in the map already. Increment the count
            search->second++;
        }
        else
        {
            // doesn't exist in the map so add
            cardMap[card] = 1;
        }
    }

    // from map, determine the hand type
    if (cardMap.size() == 1) return HandType::FiveOfAKind;
    if (cardMap.size() == 2)
    {
        // can be four of a kind or full house
        int value = cardMap.begin()->second;
        if (value == 4 || value == 1) return HandType::FourOfAKind;
        if (value == 3 || value == 2) return HandType::FullHouse;
        throw std::invalid_argument("Somehow there are 2 types but it's not Four of a Kind or Full House...");
    }
    if (cardMap.size() == 3)
    {
        // can be either Three of a Kind or Two Pair
        auto it = cardMap.begin();
        while (it != cardMap.end()) { 
            if (it->second == 3) return HandType::ThreeOfAKind;
            if (it->second == 2) return HandType::TwoPair;
            it++; 
        } 
        throw std::invalid_argument("Somehow there are 3 types but it's not Three of a Kind or Two Pair...");
    }
    if (cardMap.size() == 4) return HandType::OnePair;
    if (cardMap.size() == 5) return HandType::HighCard;
    throw std::invalid_argument("cards do not correspond to an expected hand type");
}

// sort hands in ascending order by stating if hand1 is less than hand 2
bool CompareHands(Hand hand1, Hand hand2)
{
    if (hand1.handType != hand2.handType)
    {
        return hand1.handType < hand2.handType;
    }

    // if hand types are the same, sort by card Types
    for (int i = 0; i < kNumCardsInHand; i++)
    {
        if (hand1.cards[i] != hand2.cards[i])
        {
            return hand1.cards[i] < hand2.cards[i];
        }
    }

    // if we have got through the whole for loop without returning, the cards are identical 
    // return false as this comparator is a "less than" comparator
    return false;
}

// debug methods
void PrintHand(Hand hand)
{
    cout << "Cards in Hand: ";
    for (int i = 0; i < kNumCardsInHand; i++)
    {
        std::cout << hand.cards[i] << " ";
    }
    cout << endl;

    cout << "Hand Type: " << hand.handType << endl;
    cout << "Bid: " << hand.bid << endl;
    cout << endl;
}
