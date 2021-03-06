﻿namespace NthDimension.CodeGen.Dom
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    [Serializable]
    public class DefaultMethod : AbstractMember, IMethod
    {
        IList<IParameter> parameters = null;
        IList<ITypeParameter> typeParameters = null;

        bool isExtensionMethod;

        public bool IsExtensionMethod
        {
            get
            {
                return isExtensionMethod;
            }
            set
            {
                isExtensionMethod = value;
            }
        }

        public override IMember Clone()
        {
            DefaultMethod p = new DefaultMethod(Name, ReturnType, Modifiers, Region, BodyRegion, DeclaringType);
            p.parameters = DefaultParameter.Clone(this.Parameters);
            p.typeParameters = this.typeParameters;
            p.documentationTag = DocumentationTag;
            p.isExtensionMethod = this.isExtensionMethod;
            foreach (ExplicitInterfaceImplementation eii in InterfaceImplementations)
            {
                p.InterfaceImplementations.Add(eii.Clone());
            }
            return p;
        }

        public override string DotNetName
        {
            get
            {
                if (typeParameters == null || typeParameters.Count == 0)
                    return base.DotNetName;
                else
                    return base.DotNetName + "``" + typeParameters.Count;
            }
        }

        string documentationTag;

        public override string DocumentationTag
        {
            get
            {
                if (documentationTag == null)
                {
                    string dotnetName = this.DotNetName;
                    StringBuilder b = new StringBuilder("M:", dotnetName.Length + 2);
                    b.Append(dotnetName);
                    IList<IParameter> paras = this.Parameters;
                    if (paras.Count > 0)
                    {
                        b.Append('(');
                        for (int i = 0; i < paras.Count; ++i)
                        {
                            if (i > 0) b.Append(',');
                            IReturnType rt = paras[i].ReturnType;
                            if (rt != null)
                            {
                                b.Append(rt.DotNetName);
                            }
                        }
                        b.Append(')');
                    }
                    documentationTag = b.ToString();
                }
                return documentationTag;
            }
        }

        public virtual IList<ITypeParameter> TypeParameters
        {
            get
            {
                if (typeParameters == null)
                {
                    typeParameters = new List<ITypeParameter>();
                }
                return typeParameters;
            }
            set
            {
                typeParameters = value;
            }
        }

        public virtual IList<IParameter> Parameters
        {
            get
            {
                if (parameters == null)
                {
                    parameters = new List<IParameter>();
                }
                return parameters;
            }
            set
            {
                parameters = value;
            }
        }

        public virtual bool IsConstructor
        {
            get
            {
                return ReturnType == null || Name == "#ctor";
            }
        }

        public DefaultMethod(IClass declaringType, string name) : base(declaringType, name)
        {
        }

        public DefaultMethod(string name, IReturnType type, ModifierEnum m, DomRegion region, DomRegion bodyRegion, IClass declaringType) : base(declaringType, name)
        {
            this.ReturnType = type;
            this.Region = region;
            this.BodyRegion = bodyRegion;
            Modifiers = m;
        }

        public override string ToString()
        {
            return String.Format("[AbstractMethod: FullyQualifiedName={0}, ReturnType = {1}, IsConstructor={2}, Modifier={3}]",
                                 FullyQualifiedName,
                                 ReturnType,
                                 IsConstructor,
                                 base.Modifiers);
        }

        public virtual int CompareTo(IMethod value)
        {
            int cmp;

            if (FullyQualifiedName != null)
            {
                cmp = FullyQualifiedName.CompareTo(value.FullyQualifiedName);
                if (cmp != 0)
                {
                    return cmp;
                }
            }

            cmp = this.TypeParameters.Count - value.TypeParameters.Count;
            if (cmp != 0)
            {
                return cmp;
            }

            return DiffUtility.Compare(Parameters, value.Parameters);
        }

        int IComparable.CompareTo(object value)
        {
            if (value == null)
            {
                return 0;
            }
            return CompareTo((IMethod)value);
        }
    }
}
