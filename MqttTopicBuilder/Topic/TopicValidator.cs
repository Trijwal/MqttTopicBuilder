﻿/*
 * Author
 *      Pierre Bouillon - https://github.com/pBouillon
 *
 * Repository
 *      MqttTopicBuilder - https://github.com/pBouillon/MqttTopicBuilder
 *
 * License
 *      MIT - https://github.com/pBouillon/MqttTopicBuilder/blob/master/LICENSE
 */

using MqttTopicBuilder.Constants;
using MqttTopicBuilder.Exceptions;
using MqttTopicBuilder.Exceptions.Classes;
using System.Linq;

namespace MqttTopicBuilder.Topic
{
    /// <summary>
    /// Validates MQTT topic rules
    /// </summary>
    public static class TopicValidator
    {
        /// <summary>
        /// Ensure that the provided topic is neither null, empty or blank
        /// </summary>
        /// <param name="topic">Topic to be checked</param>
        /// <exception cref="EmptyTopicException">If the topic is blank or empty</exception>
        private static void CheckNotBlank(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic)
                || topic.Contains(Mqtt.Topic.NullCharacter))
            {
                throw new EmptyTopicException();
            }
        }

        /// <summary>
        /// Validate a topic to be appended to a longer one
        /// </summary>
        /// <param name="topic">Topic value to be checked</param>
        /// <exception cref="EmptyTopicException">If the topic is blank or empty</exception>
        /// <exception cref="InvalidTopicException">If the topic is malformed</exception>
        public static void ValidateForTopicAppending(this string topic)
        {
            CheckNotBlank(topic);

            // Manually adding separators is forbidden
            if (topic.Contains(Mqtt.Topic.Separator))
            {
                throw new InvalidTopicException(
                    $"A topic should not contains the MQTT separator \"{Mqtt.Topic.Separator}\"");
            }

            // Wildcard must only be used to denote a level and shouldn't be used to denote multiple
            // characters
            if (topic.Length > 1
                && (topic.Contains(Mqtt.Wildcard.MultiLevel)
                    || topic.Contains(Mqtt.Wildcard.SingleLevel)))
            {
                throw new InvalidTopicException(
                    $"A topic value should not hold any wildcard " +
                    $"(\"{Mqtt.Wildcard.MultiLevel}\", \"{Mqtt.Wildcard.SingleLevel}\")");
            }

            // Ensure that the topic is not longer than the allowed size
            if (topic.Length > Mqtt.Topic.MaxSubTopicLength)
            {
                throw new TooLongTopicException(
                    $"Topics must not exceed {Mqtt.Topic.MaxSubTopicLength} characters (attempted to add {topic.Length})");
            }
        }

        /// <summary>
        /// Validate a fully built topic
        /// </summary>
        /// <param name="topic">Topic value to be checked</param>
        /// <exception cref="EmptyTopicException">If the topic is blank or empty</exception>
        /// <exception cref="IllegalTopicConstructionException">If the topic is abusing wildcard usage</exception>
        /// <exception cref="InvalidTopicException">If the topic is malformed</exception>
        public static void ValidateTopic(string topic)
        {
            CheckNotBlank(topic);

            // Allows minimal (and deprecated) MQTT topic: "/"
            if (topic == Mqtt.Topic.Separator.ToString())
            {
                return;
            }

            // Ensuring that the multi-level wildcard is used at most once
            var multiLevelWildcardsCount = topic.Count(_ => 
                _ == Mqtt.Wildcard.MultiLevel);

            if (multiLevelWildcardsCount > 1)
            {
                throw new IllegalTopicConstructionException(
                    ExceptionMessages.IllegalMultiLevelWildcardUsage);
            }

            // Remove trailing "/" if any
            if (topic.Last() == Mqtt.Topic.Separator)
            {
                topic = topic.Remove(topic.Length - 1);
            }

            // Ensuring that if there is any multi-level wildcard, no values are after it
            if (multiLevelWildcardsCount == 1
                && topic.Last() != Mqtt.Wildcard.MultiLevel)
            {
                throw new IllegalTopicConstructionException(
                    ExceptionMessages.TopicAfterWildcard);
            }

            // Validate each of the sub topics from which the main one is made
            topic.Split(Mqtt.Topic.Separator)
                .ToList()
                .ForEach(ValidateForTopicAppending);
        }
    }
}