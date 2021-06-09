using System.Collections.Generic;

namespace Abundance_Nk.Model.Translator
{
    public abstract class TranslatorBase<T, E>
    {
        public List<T> Translate(List<E> entities)
        {
            var models = new List<T>();
            foreach (E entity in entities)
            {
                models.Add(TranslateToModel(entity));
            }

            return models;
        }

        public T Translate(E entity)
        {
            return TranslateToModel(entity);
        }

        public E Translate(T model)
        {
            return TranslateToEntity(model);
        }

        public List<E> Translate(List<T> models)
        {
            var entities = new List<E>();
            foreach (T model in models)
            {
                entities.Add(TranslateToEntity(model));
            }

            return entities;
        }

        public abstract T TranslateToModel(E entity);
        public abstract E TranslateToEntity(T model);
    }
}