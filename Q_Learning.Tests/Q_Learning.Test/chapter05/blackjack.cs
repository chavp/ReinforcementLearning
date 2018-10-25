using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Q_Learning.Test.chapter05
{
    using Xunit;
    using Xunit.Abstractions;
    using static Q_Learning.Test.helpers.Helpers;

    public class blackjack
    {
        ITestOutputHelper console;
        public blackjack(ITestOutputHelper console)
        {
            this.console = console;
        }

        readonly static int ActionHit = 0;
        readonly static int ActionStand = 1;
        readonly static List<int> Actions = new List<int>
        {
            ActionHit, ActionStand
        };

        readonly static List<int> PolicyPlayer = null;
        readonly static List<int> PolicyDealer = null;
        static blackjack()
        {
            PolicyPlayer = Enumerable.Range(0, 22).ToList();
            PolicyDealer = Enumerable.Range(0, 22).ToList();
            for (int i = 12; i < 20; i++)
            {
                PolicyPlayer[i] = ActionHit;
                PolicyDealer[i] = ActionHit;
            }
            PolicyPlayer[20] = ActionStand;
            PolicyPlayer[21] = ActionStand;

            random = new Random();

            for (int i = 17; i < 22; i++)
            {
                PolicyDealer[i] = ActionStand;
            }
        }

        int targetPolicyPlayer(int playerSum)
        {
            return PolicyPlayer[playerSum];
        }

        static Random random;
        static int Card;
        int behaviorPolicyPlayer(int playerSum)
        {
            if (random.Next(0, 2) == 1)
                return ActionStand;
            return ActionHit;
        }

        static int getCard()
        {
            Card = random.Next(1, 14);
            Card = Math.Min(Card, 10);
            return Card;
        }

        static ((bool, int, int), int, Dictionary<(bool, int, int), int>) Play(
            Func<int, int> policyPlayer, 
            (bool, int, int)? initialState = null,
            int? initialAction = null)
        {
            int playerSum = 0;

            var playerTrajectory = new Dictionary<(bool, int, int), int>();
            bool usableAcePlayer = false;

            int dealerCard1 = 0;
            int dealerCard2 = 0;
            bool usableAceDealer = false;

            int numOfAce = 0;
            int card = 0;
            if (!initialState.HasValue)
            {
                numOfAce = 0;
                while(playerSum < 12)
                {
                    card = getCard();
                    if(card == 1)
                    {
                        numOfAce += 1;
                        card = 11;
                        usableAcePlayer = true;
                    }
                    playerSum += card;
                }

                if(playerSum > 21)
                {
                    playerSum -= 10;
                    if (numOfAce == 1)
                        usableAcePlayer = false;
                }

                dealerCard1 = getCard();
                dealerCard2 = getCard();

            }
            else
            {
                (usableAcePlayer, playerSum, dealerCard1) = initialState.Value;
                dealerCard2 = getCard();
            }

            var state = (usableAcePlayer, playerSum, dealerCard1);

            var dealerSum = 0;
            if(dealerCard1 == 1 && dealerCard2 != 1)
            {
                dealerSum += 11 + dealerCard2;
                usableAceDealer = true;
            }
            else if (dealerCard1 != 1 && dealerCard2 == 1)
            {
                dealerSum += dealerCard1 + 11;
                usableAceDealer = true;
            }
            else if (dealerCard1 == 1 && dealerCard2 == 1)
            {
                dealerSum += 1 + 11;
                usableAceDealer = true;
            }
            else
            {
                dealerSum += dealerCard1 + dealerCard2;
            }

            int action = 0;

            // player's turn
            while (true)
            {
                if (initialAction.HasValue)
                {
                    action = initialAction.Value;
                    initialAction = null;
                }
                else
                {
                    action = policyPlayer(playerSum);
                }

                playerTrajectory.Add((usableAcePlayer, playerSum, dealerCard1), action);

                if (action == ActionStand) break;

                playerSum += getCard();

                if(playerSum > 21)
                {
                    if (usableAcePlayer)
                    {
                        playerSum -= 10;
                        usableAcePlayer = false;
                    }
                    else
                    {
                        return (state, -1, playerTrajectory);
                    }
                }
            }

            // dealer's turn
            while (true)
            {
                action = PolicyDealer[dealerSum];
                if (action == ActionStand)
                    break;

                var newCard = getCard();
                if(newCard == 1 && dealerSum + 11 < 21)
                {
                    dealerSum += 11;
                    usableAceDealer = true;
                }
                else
                {
                    dealerSum += newCard;
                }

                if(dealerSum > 21)
                {
                    if (usableAceDealer)
                    {
                        dealerSum -= 10;
                        usableAceDealer = false;
                    }
                    else
                    {
                        return (state, 1, playerTrajectory);
                    }
                }
            }

            // compare the sum between player and dealer
            if (playerSum > dealerSum)
                return (state, 1, playerTrajectory);
            else
                return (state, -1, playerTrajectory);
        }

        // Monte Carlo Sample with On-Policy
        (Dictionary<(int, int), double>, Dictionary<(int, int), double>) 
            monte_carlo_on_policy(int episodes)
        {
            var states_usable_ace = new Dictionary<(int, int), double>();
            var states_usable_ace_count = new Dictionary<(int, int), double>();
            var states_no_usable_ace = new Dictionary<(int, int), double>();
            var states_no_usable_ace_count = new Dictionary<(int, int), double>();

            ForEachLoop(10, 10, (i, j) =>
            {
                states_usable_ace.Add((i, j), 0);
                states_usable_ace_count.Add((i, j), 1);
                states_no_usable_ace.Add((i, j), 0);
                states_no_usable_ace_count.Add((i, j), 1);
            });

            //int playerSum = 0;
            //int dealerCard = 0;
            for (int i = 0; i < episodes; i++)
            {
                ((bool, int, int)_, int reward, Dictionary<(bool, int, int), int> player_trajectory) 
                    = Play(targetPolicyPlayer);

                foreach (((bool usable_ace, int player_sum, int dealer_card), int act) in player_trajectory)
                {
                    var r_player_sum = player_sum - 12;
                    var r_dealer_card = dealer_card - 1;
                    if (usable_ace)
                    {
                        states_usable_ace_count[(r_player_sum, r_dealer_card)] += 1;
                        states_usable_ace[(r_player_sum, r_dealer_card)] += reward;
                    }
                    else
                    {
                        states_no_usable_ace_count[(r_player_sum, r_dealer_card)] += 1;
                        states_no_usable_ace[(r_player_sum, r_dealer_card)] += reward;
                    }
                }
            }

            var r_states_usable_ace = new Dictionary<(int, int), double>();
            var r_states_no_usable_ace = new Dictionary<(int, int), double>();
            ForEachLoop(10, 10, (i, j) =>
            {
                r_states_usable_ace.Add((i, j), states_usable_ace[(i, j)] / states_usable_ace_count[(i, j)]);
                r_states_no_usable_ace.Add((i, j), states_no_usable_ace[(i, j)] / states_no_usable_ace_count[(i, j)]);

            });

            return (r_states_usable_ace, r_states_no_usable_ace);
        }

        // Monte Carlo with Exploring Starts

        [Fact]
        public void figure_5_1()
        {
            (Dictionary<(int, int), double> states_usable_ace_1, Dictionary<(int, int), double> states_no_usable_ace_1) 
                = monte_carlo_on_policy(10000);
            (Dictionary<(int, int), double> states_usable_ace_2, Dictionary<(int, int), double> states_no_usable_ace_2) 
                = monte_carlo_on_policy(500000);
        }
    }
}
