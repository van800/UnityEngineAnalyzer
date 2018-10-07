﻿using Linty.Analyzers;
using Linty.Analyzers.Transform;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using RoslynNUnitLight;

namespace UnityEngineAnalyzer.Test.Transform
{
    [TestFixture]
    sealed class InstantiateAnalyzerTests : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new InstantiateAnalyzer();

        [Test]
        public void IfParentIsSetRightAfterInstantiateRaiseWarningNewVariable()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    GameObject prefabObject;
    GameObject newParent;

    void Update()
    {
        var newGameobject = [|Instantiate(prefabObject, Vector3.zero, Quaternion.identity)|];
        newGameobject.transform.SetParent(newParent.transform, false);
    }
}";

            HasDiagnostic(code, DiagnosticIDs.InstantiateShouldTakeParentArgument);
        }

        [Test]
        public void IfParentIsSetRightAfterInstantiateRaiseWarningOldVariable()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    GameObject prefabObject;
    GameObject newParent;
    GameObject newGameObject;

    void Update()
    {
        newGameObject = [|Instantiate(prefabObject, Vector3.zero, Quaternion.identity)|];
        newGameObject.transform.SetParent(newParent.transform, false);
    }
}";

            HasDiagnostic(code, DiagnosticIDs.InstantiateShouldTakeParentArgument);
        }

        [Test]
        public void IfParentIsSetRightAfterInstantiateRaiseWarningOldVariableAsTransform()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    GameObject prefabObject;
    GameObject newParent;
    Transform newTransform;

    void Update()
    {
        newTransform = [|Instantiate(prefabObject, Vector3.zero, Quaternion.identity)|] as Transform;
        newTransform.SetParent(newParent.transform, false);
    }
}";

            HasDiagnostic(code, DiagnosticIDs.InstantiateShouldTakeParentArgument);
        }


        [Test]
        public void Instantiate_Should_Not_Throw_Warning_If_Parent_Is_Set_On_Instantiate()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    GameObject prefabObject;
    GameObject newParent;

    void Update()
    {
        [|var newGameobject = Instantiate(prefabObject, newParent.transform);|]
    }
}";

            NoDiagnostic(code, DiagnosticIDs.InstantiateShouldTakeParentArgument);
        }

        [Test]
        public void Instantiate_Should_Not_Throw_Warning_If_SetParent_Is_Not_Called()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    GameObject prefabObject;
    GameObject newParent;

    void Update()
    {
        [|var newGameobject = Instantiate(prefabObject);|]
        newParent.transform.SetParent(newGameobject.transform, false);
    }
}";

            NoDiagnostic(code, DiagnosticIDs.InstantiateShouldTakeParentArgument);
        }

        [Test]
        public void Instantiate_Should_Not_Throw_Warning_If_SetParent_Is_Called_Before_Instantiate()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    GameObject prefabObject;
    GameObject newParent;
    GameObject newGameobject;

    void Update()
    {
        newGameobject.transform.SetParent(newParent.transform, false);
        [|var newGameobject = Instantiate(prefabObject);|]
    }
}";

            NoDiagnostic(code, DiagnosticIDs.InstantiateShouldTakeParentArgument);
        }

        [Test]
        public void Instantiate_Should_Not_Throw_Warning_If_SetParent_Is_Called_On_Different_Variable()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    GameObject prefabObject;
    GameObject newParent;

    void Update()
    {
        [|var newGameobject = Instantiate(prefabObject);|]
        newGameObject = prefabObject;
        newGameobject.transform.SetParent(newParent.transform, false);
    }
}";

            NoDiagnostic(code, DiagnosticIDs.InstantiateShouldTakeParentArgument);
        }
    }
}
