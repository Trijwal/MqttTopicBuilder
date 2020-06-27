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

using System.Linq;
using MqttTopicBuilder.Constants;

namespace MqttTopicBuilder.Topic
{
    /// <summary>
    /// Represent an MQTT topic
    /// </summary>
    public class Topic
    {
        /// <summary>
        /// Number of levels of this topic
        /// </summary>
        public int Levels
            => Value == Mqtt.Topic.Separator.ToString()
            // If the topic is the smallest one allowed, its level is one
            ? 1
            // Otherwise, its the number of parts of the topic
            : Value.Split(Mqtt.Topic.Separator)
                .Length;

        /// <summary>
        /// Value of the MQTT topic (e.g "a/b/c")
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// Create a new MQTT Topic from a raw string
        /// <para>
        /// If <paramref name="rawTopic"/> is empty, the minimal topic will be created 
        /// (made only of a single <see cref="Mqtt.Topic.Separator"/>)
        /// </para>
        /// </summary>
        /// <param name="rawTopic">Raw MQTT topic</param>
        /// <remarks>
        /// Any trailing separator will be removed
        /// </remarks>
        public Topic(string rawTopic)
        {
            // Create minimal topic on empty string or already minimal raw string
            if (string.IsNullOrEmpty(rawTopic)
                || rawTopic == Mqtt.Topic.Separator.ToString())
            {
                Value = Mqtt.Topic.Separator.ToString();
                return;
            }

            TopicValidator.ValidateTopic(rawTopic);

            // Remove trailing "/" if any
            if (rawTopic.Last() == Mqtt.Topic.Separator)
            {
                rawTopic = rawTopic.Remove(rawTopic.Length - 1);
            }

            Value = rawTopic;
        }

        /// <summary>
        /// Static method to create a new MQTT Topic from a raw string
        /// </summary>
        /// <param name="rawTopic">Raw MQTT topic</param>
        /// <remarks>
        /// The raw string will be validated beforehand using <see cref="TopicValidator"/> methods
        /// </remarks>
        /// <returns>A new instance of the Topic</returns>
        public static Topic FromString(string rawTopic)
            => new Topic(rawTopic);

        /// <summary>
        /// Returns the topic's value as a string, same as as <see cref="Value"/>
        /// </summary>
        /// <returns>The topic's value</returns>
        public override string ToString()
            => Value;
    }
}
