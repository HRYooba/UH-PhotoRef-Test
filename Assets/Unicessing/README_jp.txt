                                                                    @mainpage
Unicessing
======================

Ver.0.16

Processing like Drawing API

@image html unicessing.jpg



概要
--------

Unityで四角や丸などの図形を手軽に描くライブラリ（アセット）一式です。
Processingに似た簡単なAPIを使い、内部的にDrawMesh()で描くのが特徴です。
Unity 5.4以上を対象としています。

使い方は簡単、UGraphicsを継承したスクリプトをオブジェクトに貼り付け、
Setup()で初期化、Draw()で図形を描きます。Unityの座標系です。
Unityのコードもそのまま共存して使えます。言語はC#です。

サンプルを参考にして、短いコードで動的な図形描画（ジェネラティブアート）を楽しみましょう。
Oculus Rift、HTC ViveなどのVR HMD上での動作も確認済みです。



                                                                    @page 更新履歴
更新履歴
--------

### Ver.0.16 / 2016.12.22

Add reference.
* UGraphicsにリファレンス用の簡単なコメントを追加。オンラインリファレンスを公開。
* 日本語 http://dev.eyln.com/unicessing/doc/jp/
* 英語 http://dev.eyln.com/unicessing/doc/en/

Change functions.
* stroke(), fill(), background()の引数をintからfloatに変更（intもfloatになるので両方OK）。
* background(CameraClearFlags flags)をbackgroundFlags(CameraClearFlags flags)に変更。
* beginShape()のデフォルト値をLINE_STRIPに指定。
* color()をUConstansのstaticから通常の関数に変更。
* rotateDegree()をrotateDegrees()に改名。
* getWorldPos(localPos)をlocalToWorldPos(localPos)に改名。
* getLocalPos(worldPos)をworldToLocalPos(worldPos)に改名。
* getWorldDir()をlocalToWorldDir()に、getLocalDir()をworldToLocalDir()に改名。
* clearDepthStep()を廃止して、同機能をnoDepthStep()に統一。

Add functions.
* backgroundSkybox()を追加。
* colorMode()を追加。RGBまたはHSB色空間と色のスケールを設定可能。
* 色関係の関数 red(), blue(), green(), alpha(), hue(), saturation(), brightness(), lerpColor()を追加。
* UShape addChild(), removeChild()を追加。
* rotate(xAngle, yAngle, zAngle)、rotateDegrees(eulerAngles)を追加。


### Ver.0.15 / 2016.12.12

Bug fix.
* beginShape(QUAD)でポリゴンが欠けないように修正。
* ellipse()のサイズが2倍になっていたのを修正。
* P2D、P3Dモード時のmouseX、Y、Z値を修正。
* P2DのときはデフォルトでnoLights()になるように変更。

Add functions.
* UShapeにlights(), noLights()を追加。


### Ver.0.14 / 2016.12.8

Bug fix.
* scale()したときのtranslate()関連の計算を修正。
* text()でscale()が効くように対応。text()でz値指定も可。
* UGraphicsをつけたGameObjectの位置や向きを変えたときの挙動を修正。
* Shaderを両面描画に変更。

Add functions.
* size()でProcessing座標軸を設定できるように対応（β版）。
  width、heightはデフォルト1で、size()で設定可能。
* triangle()を追加。
* Processing座標系を使ったサンプルP5_Snowsを追加。
* テキストを周囲に配置するサンプルZenTextsを追加。


### Ver.0.13 / 2016.11.9

First release.
* 迷路をカーソルキーで進むMazeサンプルを追加。
* USubGraphicsを使ったRunnerサンプルを追加。
* 全体を少し整理。


### Ver.0.12 / 2016.11.4

Add functions.
* rectMode(), ellipseMode(), imageMode()による基準位置変更に対応。
* mesh()でSubMeshも描画するように対応。
* stroke()でプリミティブのワイヤーフレーム表示に対応。
* noStroke(), noFill()に対応。


### Ver.0.11 / 2016.10.29

Bug fix.
* loadImage()でWWW経由のダウンロードに対応。
* randomSeed()の初期化をUnityのバージョン別にあわせる修正。


### Ver.0.10 / 2016.10.29

Alpha test.
* α版一式を作成＆公開。



                                                                    @page 必要アセット
必要アセット
---------------

同梱のサンプルではUnity社が提供している別のアセットを使います。
お手数ですが、下記のアセットを事前にインポートしておいてください。


[1]. シネマティックイメージエフェクト

https://www.assetstore.unity3d.com/jp/#!/content/51515

上記アセットをAsset Storeから入手してください。
カメラのImageEffectとしてBloomとTonemappingColorGradingだけを使用しています。


[2]. Third Person Character（Ethan）

Runnnerサンプルでキャラクターとして使用しています。
Unityのメニューから Assets -> Import Package -> Character を選び
Third Person Characterにチェックを入れてImportしてください。



                                                                    @page サンプル
サンプル
-------

Assets/Unicessing/Scenesに各種サンプルシーンが入っています。
Menuのシーンから各シーンを呼び出せて、各シーンでEnterかBSキーを押すと前のシーンに戻れます。

サンプルのソースコードはUnicessing/Scripts/Samplesにあります。

各種オーバーライド関数を含んだテンプレートとして使えるソースはUnicessing/Scripts/Samples/UnicessingTemplate.csです。

※Menuサンプルで各シーンをロードするには、Build Settingsに各サンプルシーンを追加しておく必要があります。


### 追加のサンプル

Processingのいくつかのコードを移植したサンプルを、下記にて公開しています。

http://eyln.hatenablog.com/entry/2016/12/15/100335



                                                                    @page 説明
説明
------------

### オンラインリファレンス

UGraphicsを中心とした簡単なオンラインリファレンスはこちら

* 日本語 http://dev.eyln.com/unicessing/doc/jp/
* 英語 http://dev.eyln.com/unicessing/doc/en/

Unicessingが参考にしたProcessing本体のAPIリファレンスはこちら
（サポートしている関数はこの一部です）

* https://processing.org/reference/


### 関数概要とProcessingとの比較

座標系はUnityの座標系にあわせているので、Processingと違って上方向がY+です。
rect()の基準位置は左下です。

※実験的に、size(640, 480, P3D, 0.01f)などの指定を行うと、Processingの座標系に
変更できるようにしました。Unityの座標と混在すると混乱するので推奨はしませんが、
Processingのコードを移植するのは楽になるかもしれません。
width、heightはsize()で指定した値をそのまま返します。
最後の0.01fは全体にかかるスケール値です。

※camera()はないので、Unityのヒエラルキー上で見え方を調整しましょう。

線や点はline()、curve()、point()で描けます。線の色はstroke()で指定します。
strokeWeight()による線の太さの変更は現在未サポートです。

四角はrect()、丸はellipse()、箱はbox()、球はsphere()で描けます。
塗りつぶす色はfill()で指定します。noFill()だと塗りつぶしません。

stroke()でstroke色を指定してあると、rect()、ellipse()のエッジや、box()、sphere()などの
ワイヤーフレーム表示ができます。noStroke()でワイヤーフレーム表示OFFです。

rect()はrectMode()やellipse()はellipseMode()で基準位置を変更できます。
デフォルトはrect()がCORNER=左下、ellipse()はCENTER=中心です。CORNER_P5で左上基準になります。

色の処理はUnityのColorをそのまま使える他、
color()で色を作り、色成分をred()、blue()、green()、alpha()、
あるいは色相、彩度、明度を、hue()、saturation()、brightness()で取り出せます。
lerpColor()で色を補間できます。

colorMode()を使うと色空間をRGBからHSB(HSV)に変えたり、
色の単位をデフォルトの０~255から0.0f～1.0fにしたりもできます。

noLigths()でシェーディングなし、lights()でシェーディングありです。

blendMode()で不透明、透明色あり、加算などの切り替えができます。

文字はtext()です。textAlign()で基準位置、textSize()で文字の大きさ、
textFont()でフォント変更もできます。
フォントを読み込んでおくloadFont()もあります。

画像はloadImage()したあと、image()で描けます。
imageMode()で基準位置を変更できます。

texture()で画像を指定すると図形をそのテクスチャ画像で塗りつぶします。
noTexture()で色のみに戻ります。

座標移動はtranslate()、回転はrotate()、スケール変更はscale()です。
なお、回転についてはデフォルトがラジアン単位です。
度単位で指定したい場合はrotate(radians(90))のようにするか、
事前にrotateDegrees()を呼んでおいて度単位モードにしておき、rotate(90)のように使うとよいです。

行列をスタックに入れたり取り出したりするpushMatrix()、popMatrix()も使えます。
色などを同様に入れたり取り出したりするのはpushStyle()、popStyle()です。
両方をやってくれるのがpush()、pop()です。

マウス位置はmouseX、mouseYの他、mouse3DでVector3の値も取れます。
指定したゲームオブジェクトのforward方向を法線とした仮想平面とのあたり判定をとっています。
mousePressed、mouseReleased、mouseButtonも使えます。

キー入力はisKey()、isKeyDown()、isKeyUp()で確認できます。

任意の図形はbeginShape()～endShape()の間にvertex()で頂点指定することで描けます。
あらかじめcreateShape()して作成したUShape型のクラスに対して同様に頂点を設定して、
draw()で描くのも手です。

その他、各種数学関数も使えます。


### 拡張

loadPrefab()でPrefabをロード、loadScene()でシーンをロード、
lookAt()でカメラの方を見たり、
createObject()でオブジェクトを作成して非表示にしておき、draw()で表示したり、
mesh()でメッシュを描画したり、といったUnity的な便利関数も一部追加しています。

Unicessingは複数同時に動かせますが、USubGraphicsを使うとひとつのUGraphicsを
複数のGameObjectのCompornentで共有して描画に使ったりできます。
これらの使用例はUnicessingMenu.csやUnicessingRunnner.csにあります。

isVRでVRモードかどうかも判断できます。
VRモードではmouseX、mouseY、mouse3Dの値が、HMDで見ている方向の中心位置になります。

その他の関数については、Coreのコードをご覧ください。
Unicessing/Scripts/Core/UGraphics.cs and Core/*.cs


### uProcessingとの違い

Unity上でProcessingのように書ける似たライブラリとして
uProcessingというものを以前作りました。

そちらは１つの図形ごとにGameObjectとして作るタイプで、
size()、camera()、strokeWeight()などでProcessingとの互換性を意識しています。

UnicessingはUnity上での使いやすさを主眼にしつつ、
Processing風味があるというバランスです。
Unicessingの方が新しいので、関数も豊富です。

描画の仕組みが違うため、UnicessingはuProcessingより大分高速です。
ただし、そこまで高速化は行っていないので、
簡単なジェネラティブアートで遊んだり、部分的な演出として使ったり、
プロトタイピングしたりするのに向いています。

本格的に多数のオブジェクトを描画したい場合には専用にShaderを書いたり、
Meshを結合して描画するようにしたり、uRaymachingを使用したりと、
やりたいことにあわせて個別に対処した方がよいでしょう。



ご注意
-----------

UnicessingにはProcessingにある関数と仕様が異なったり、未定義のものがあります。
また今後予告なく仕様を変更することがあります。



Copyright
----------

### Unicessing

Copyright (C) 2016 NISHIDA Ryota / ship of EYLN

http://dev.eyln.com/

