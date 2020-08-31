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

using MqttTopicBuilder.Collection;
using MqttTopicBuilder.Exceptions.Classes;
using TinyValidator.Abstractions;

namespace MqttTopicBuilder.Validators.Rules.ITopicCollectionRules
{
    /// <summary>
    /// Rule to ensure that the collection is able to hold another topic
    /// </summary>
    public class MustNotBeFull : BaseITopicCollectionRule
    {
        /// <inheritdoc cref="Rule{T}.IsValidWhen"/>
        protected override bool IsValidWhen(ITopicCollection value)
            => value.Levels < value.MaxLevel;

        /// <inheritdoc cref="Rule{T}.OnInvalid"/>
        protected override void OnInvalid()
            => throw new TooManyTopicsAppendingException();
    }
}
