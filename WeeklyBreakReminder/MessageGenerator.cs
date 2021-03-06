﻿using System;

namespace WeeklyBreakReminder
{
    class MessageGenerator
    {

        public MessageGenerator()
        {

        }

        public String GenerateMessage(int interval, int day, bool compact)
        {
            String announcementMessage;
            String breakMessage = "This might be a good time to take a break.";
            if (day % 7 == 1 && interval % 7 == 0)
            {
                // Special message for Mondays, which are the start of the week in Stardew Valley
                announcementMessage = "It's the start of a new week!";
            }
            else
            {
                if (interval % 7 == 0)
                {
                    if (interval > 7)
                    {
                        announcementMessage = $"Another {GetNumName(interval / 7)} weeks have passed!";
                    }
                    else
                    {
                        announcementMessage = "Another week has passed!";
                    }
                }
                else if (interval <= 10)
                {
                    if (interval > 1)
                    {
                        announcementMessage = $"Another {GetNumName(interval)} days have passed!";
                    }
                    else
                    {
                        announcementMessage = $"Another day has passed!";
                    }
                }
                else
                {
                    announcementMessage = $"Another {interval} days have passed!";
                }
            }

            if (compact)
            {
                return $"{announcementMessage}^ {breakMessage} ";
            }
            else
            {
                return $"^{announcementMessage} ^^{breakMessage}^";
            }
        }

        public String GenerateStartupMessage(int interval, String dayOfWeek)
        {
            if (interval == 1)
            {
                return "You will be reminded to take a break every day.";
            }
            else if (interval % 7 == 0)
            {
                return $"You will be reminded to take a break on the next {dayOfWeek}.";
            }
            else
            {
                return $"You will be reminded to take a break every {interval} days.";
            }
        }

        public String GetNumName(int num)
        {
            String[] numeralName = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten" };
            return numeralName[num];
        }
    }
}
